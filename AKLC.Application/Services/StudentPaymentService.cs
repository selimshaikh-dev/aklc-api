using AKLC.Application.DTOs.StudentPayments;
using AKLC.Application.Interfaces;
using AKLC.Domain.Entities;

namespace AKLC.Application.Services
{
    public class StudentPaymentService
        : IStudentPaymentService
    {
        private readonly IStudentPaymentRepository
            _studentPaymentRepository;

        private readonly IStudentRepository
            _studentRepository;

        private readonly IPaymentScheduleRepository
            _paymentScheduleRepository;

        private readonly IPaymentAllocationRepository
            _paymentAllocationRepository;

        private readonly IStudentFeeAssignmentRepository
            _studentFeeAssignmentRepository;


        // =========================================
        // CONSTRUCTOR
        // =========================================

        public StudentPaymentService(
            IStudentPaymentRepository studentPaymentRepository,
            IStudentRepository studentRepository,
            IPaymentScheduleRepository paymentScheduleRepository,
            IPaymentAllocationRepository paymentAllocationRepository,
            IStudentFeeAssignmentRepository studentFeeAssignmentRepository)
        {
            _studentPaymentRepository =
                studentPaymentRepository;

            _studentRepository =
                studentRepository;

            _paymentScheduleRepository =
                paymentScheduleRepository;

            _paymentAllocationRepository =
                paymentAllocationRepository;

            _studentFeeAssignmentRepository =
                studentFeeAssignmentRepository;
        }


        // =========================================
        // CREATE PAYMENT
        // =========================================

        public async Task<StudentPaymentDto> CreateAsync(
            CreateStudentPaymentRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request.StudentId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Student is required.");
            }

            if (request.Amount <= 0)
            {
                throw new ArgumentException(
                    "Payment amount must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(
                request.PaymentMethod))
            {
                throw new ArgumentException(
                    "Payment method is required.");
            }

            var student =
                await _studentRepository
                    .GetByIdForUpdateAsync(
                        request.StudentId,
                        cancellationToken);

            if (student is null)
            {
                throw new InvalidOperationException(
                    "Student was not found.");
            }

            if (!student.IsActive)
            {
                throw new InvalidOperationException(
                    "Payment cannot be received for an inactive student.");
            }


            // =========================================
            // CURRENT FINANCIAL POSITION
            // BEFORE THIS NEW PAYMENT
            // =========================================

            var totalPayable =
                await _studentFeeAssignmentRepository
                    .GetTotalNetPayableAsync(
                        student.Id,
                        cancellationToken);

            var totalValidPaid =
                await _studentPaymentRepository
                    .GetTotalValidPaidAsync(
                        student.Id,
                        cancellationToken);

            var currentOutstanding =
                Math.Max(
                    totalPayable - totalValidPaid,
                    0m);

            var outstandingAfterPayment =
                Math.Max(
                    currentOutstanding - request.Amount,
                    0m);


            var paymentDate =
                request.PaymentDate == default
                    ? DateTime.UtcNow
                    : request.PaymentDate;

            var receiptNumber =
                GenerateReceiptNumber();

            var payment =
                new StudentPayment
                {
                    Id =
                        Guid.NewGuid(),

                    StudentId =
                        student.Id,

                    ReceiptNumber =
                        receiptNumber,

                    Amount =
                        request.Amount,

                    PaymentDate =
                        paymentDate,

                    PaymentMethod =
                        request.PaymentMethod.Trim(),

                    ReferenceNumber =
                        Normalize(
                            request.ReferenceNumber),

                    Remarks =
                        Normalize(
                            request.Remarks),

                    IsReversed =
                        false,

                    ReversedAt =
                        null,

                    ReversalReason =
                        null,

                    ReversedByUserId =
                        null,

                    CreatedAt =
                        DateTime.UtcNow
                };


            // =========================================
            // CREATE PAYMENT ATTACHMENTS
            // =========================================

            if (request.Attachments is not null)
            {
                foreach (var item in request.Attachments)
                {
                    if (item is null)
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(
                        item.FilePath))
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(
                        item.OriginalFileName))
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(
                        item.StoredFileName))
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(
                        item.ContentType))
                    {
                        continue;
                    }

                    if (item.FileSize <= 0)
                    {
                        continue;
                    }

                    var attachment =
                        new PaymentAttachment
                        {
                            Id =
                                Guid.NewGuid(),

                            StudentPaymentId =
                                payment.Id,

                            OriginalFileName =
                                item.OriginalFileName.Trim(),

                            StoredFileName =
                                item.StoredFileName.Trim(),

                            FilePath =
                                item.FilePath.Trim(),

                            ContentType =
                                item.ContentType.Trim(),

                            FileSize =
                                item.FileSize,

                            Description =
                                Normalize(
                                    item.Description),

                            IsDeleted =
                                false,

                            CreatedAt =
                                DateTime.UtcNow
                        };

                    payment.Attachments.Add(
                        attachment);
                }
            }


            // =========================================
            // ADD PAYMENT
            // =========================================

            await _studentPaymentRepository
                .AddAsync(
                    payment,
                    cancellationToken);


            // =========================================
            // AUTO ALLOCATION
            // =========================================

            var schedules =
                await _paymentScheduleRepository
                    .GetByStudentIdAsync(
                        student.Id,
                        cancellationToken);

            var activeSchedules =
                schedules
                    .Where(x =>
                        x.IsActive &&
                        !x.IsDeleted)
                    .OrderBy(x =>
                        x.DueDate)
                    .ThenBy(x =>
                        x.CreatedAt)
                    .ToList();

            var remainingPaymentAmount =
                payment.Amount;

            var remainingScheduledBalanceAfterPayment =
                0m;

            foreach (var schedule in activeSchedules)
            {
                var alreadyAllocatedAmount =
                    await _paymentAllocationRepository
                        .GetValidAllocatedAmountAsync(
                            schedule.Id,
                            cancellationToken);

                var scheduleRemainingAmount =
                    Math.Max(
                        schedule.ScheduledAmount -
                        alreadyAllocatedAmount,
                        0m);

                if (scheduleRemainingAmount <= 0)
                {
                    continue;
                }

                var amountToAllocate =
                    remainingPaymentAmount > 0
                        ? Math.Min(
                            remainingPaymentAmount,
                            scheduleRemainingAmount)
                        : 0m;

                if (amountToAllocate > 0)
                {
                    var allocation =
                        new PaymentAllocation
                        {
                            Id =
                                Guid.NewGuid(),

                            StudentPaymentId =
                                payment.Id,

                            PaymentScheduleId =
                                schedule.Id,

                            AllocatedAmount =
                                amountToAllocate,

                            Remarks =
                                "Auto allocated during payment receipt.",

                            CreatedAt =
                                DateTime.UtcNow
                        };

                    payment.Allocations.Add(
                        allocation);

                    remainingPaymentAmount -=
                        amountToAllocate;
                }

                remainingScheduledBalanceAfterPayment +=
                    Math.Max(
                        scheduleRemainingAmount -
                        amountToAllocate,
                        0m);
            }


            // =========================================
            // CREATE NEXT PAYMENT SCHEDULE
            //
            // Only create a new schedule for the part
            // of the remaining outstanding balance that
            // is not already covered by an active
            // payment schedule.
            //
            // The new payment is NOT allocated to this
            // new schedule because it represents the
            // future remaining balance.
            // =========================================

            var uncoveredOutstanding =
                Math.Max(
                    outstandingAfterPayment -
                    remainingScheduledBalanceAfterPayment,
                    0m);

            if (uncoveredOutstanding > 0)
            {
                if (!request.NextPaymentDueDate.HasValue)
                {
                    throw new InvalidOperationException(
                        "Next payment due date is required when a balance remains unpaid.");
                }

                var paymentBusinessDate =
                    DateOnly.FromDateTime(
                        paymentDate);

                if (request.NextPaymentDueDate.Value <=
                    paymentBusinessDate)
                {
                    throw new InvalidOperationException(
                        "Next payment due date must be after the payment date.");
                }

                var nextSchedule =
                    new PaymentSchedule
                    {
                        Id =
                            Guid.NewGuid(),

                        StudentId =
                            student.Id,

                        StudentFeeAssignmentId =
                            null,

                        Title =
                            "Remaining Payment",

                        ScheduledAmount =
                            uncoveredOutstanding,

                        DueDate =
                            request.NextPaymentDueDate.Value,

                        Remarks =
                            "Automatically created for the remaining outstanding balance after partial payment.",

                        IsActive =
                            true,

                        IsDeleted =
                            false,

                        CreatedAt =
                            DateTime.UtcNow
                    };

                await _paymentScheduleRepository
                    .AddAsync(
                        nextSchedule,
                        cancellationToken);
            }


            // =========================================
            // SINGLE DATABASE SAVE
            // =========================================

            await _studentPaymentRepository
                .SaveChangesAsync(
                    cancellationToken);

            return MapToDto(
                payment);
        }


        // =========================================
        // GET BY ID
        // =========================================

        public async Task<StudentPaymentDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException(
                    "Payment id is required.");
            }

            var payment =
                await _studentPaymentRepository
                    .GetByIdAsync(
                        id,
                        cancellationToken);

            return payment is null
                ? null
                : MapToDto(
                    payment);
        }


        // =========================================
        // GET BY STUDENT
        // =========================================

        public async Task<IReadOnlyList<StudentPaymentDto>>
            GetByStudentIdAsync(
                Guid studentId,
                CancellationToken cancellationToken = default)
        {
            if (studentId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Student is required.");
            }

            var payments =
                await _studentPaymentRepository
                    .GetByStudentIdAsync(
                        studentId,
                        cancellationToken);

            return payments
                .Select(MapToDto)
                .ToList();
        }


        // =========================================
        // REVERSE PAYMENT
        // =========================================

        public async Task<StudentPaymentDto> ReverseAsync(
            Guid paymentId,
            ReverseStudentPaymentRequest request,
            Guid reversedByUserId,
            CancellationToken cancellationToken = default)
        {
            if (paymentId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Payment id is required.");
            }

            if (reversedByUserId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Reversing user is required.");
            }

            if (string.IsNullOrWhiteSpace(
                request.Reason))
            {
                throw new ArgumentException(
                    "Reversal reason is required.");
            }

            var payment =
                await _studentPaymentRepository
                    .GetByIdForUpdateAsync(
                        paymentId,
                        cancellationToken);

            if (payment is null)
            {
                throw new InvalidOperationException(
                    "Payment was not found.");
            }

            if (payment.IsReversed)
            {
                throw new InvalidOperationException(
                    "Payment has already been reversed.");
            }

            var reversedAt =
                DateTime.UtcNow;

            payment.IsReversed =
                true;

            payment.ReversedAt =
                reversedAt;

            payment.ReversalReason =
                request.Reason.Trim();

            payment.ReversedByUserId =
                reversedByUserId;

            payment.UpdatedAt =
                reversedAt;

            await _studentPaymentRepository
                .SaveChangesAsync(
                    cancellationToken);

            return MapToDto(
                payment);
        }


        // =========================================
        // MAP ENTITY TO DTO
        // =========================================

        private static StudentPaymentDto MapToDto(
            StudentPayment payment)
        {
            return new StudentPaymentDto
            {
                Id =
                    payment.Id,

                StudentId =
                    payment.StudentId,

                ReceiptNumber =
                    payment.ReceiptNumber,

                Amount =
                    payment.Amount,

                PaymentDate =
                    payment.PaymentDate,

                PaymentMethod =
                    payment.PaymentMethod,

                ReferenceNumber =
                    payment.ReferenceNumber,

                Remarks =
                    payment.Remarks,

                IsReversed =
                    payment.IsReversed,

                ReversedAt =
                    payment.ReversedAt,

                ReversalReason =
                    payment.ReversalReason,

                CreatedAt =
                    payment.CreatedAt,

                Attachments =
                    payment.Attachments
                        .Where(x =>
                            !x.IsDeleted)
                        .OrderBy(x =>
                            x.CreatedAt)
                        .Select(x =>
                            new PaymentAttachmentDto
                            {
                                Id =
                                    x.Id,

                                OriginalFileName =
                                    x.OriginalFileName,

                                StoredFileName =
                                    x.StoredFileName,

                                FilePath =
                                    x.FilePath,

                                ContentType =
                                    x.ContentType,

                                FileSize =
                                    x.FileSize,

                                Description =
                                    x.Description,

                                CreatedAt =
                                    x.CreatedAt
                            })
                        .ToList()
            };
        }


        // =========================================
        // GENERATE RECEIPT NUMBER
        // =========================================

        private static string GenerateReceiptNumber()
        {
            return
                $"RCP-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
        }


        // =========================================
        // NORMALIZE OPTIONAL STRING
        // =========================================

        private static string? Normalize(
            string? value)
        {
            return string.IsNullOrWhiteSpace(
                value)
                ? null
                : value.Trim();
        }
    }
}