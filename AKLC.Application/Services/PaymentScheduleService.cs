using AKLC.Application.DTOs.PaymentSchedules;
using AKLC.Application.Interfaces;
using AKLC.Domain.Entities;

namespace AKLC.Application.Services
{
    public class PaymentScheduleService
        : IPaymentScheduleService
    {
        private readonly IPaymentScheduleRepository
            _paymentScheduleRepository;

        private readonly IStudentRepository
            _studentRepository;

        private readonly IStudentFeeAssignmentRepository
            _studentFeeAssignmentRepository;

        private readonly IStudentPaymentRepository
            _studentPaymentRepository;

        private readonly IPaymentAllocationRepository
            _paymentAllocationRepository;


        // =========================================
        // CONSTRUCTOR
        // =========================================

        public PaymentScheduleService(
            IPaymentScheduleRepository paymentScheduleRepository,
            IStudentRepository studentRepository,
            IStudentFeeAssignmentRepository studentFeeAssignmentRepository,
            IStudentPaymentRepository studentPaymentRepository,
            IPaymentAllocationRepository paymentAllocationRepository)
        {
            _paymentScheduleRepository =
                paymentScheduleRepository;

            _studentRepository =
                studentRepository;

            _studentFeeAssignmentRepository =
                studentFeeAssignmentRepository;

            _studentPaymentRepository =
                studentPaymentRepository;

            _paymentAllocationRepository =
                paymentAllocationRepository;
        }


        // =========================================
        // CREATE PAYMENT SCHEDULE
        // =========================================

        public async Task<PaymentScheduleDto>
            CreateAsync(
                CreatePaymentScheduleRequest request,
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // BASIC VALIDATION
            // =====================================

            if (request.StudentId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Student is required.");
            }

            if (string.IsNullOrWhiteSpace(
                request.Title))
            {
                throw new ArgumentException(
                    "Schedule title is required.");
            }

            if (request.ScheduledAmount <= 0)
            {
                throw new ArgumentException(
                    "Scheduled amount must be greater than zero.");
            }

            if (request.DueDate == default)
            {
                throw new ArgumentException(
                    "Due date is required.");
            }


            // =====================================
            // VALIDATE STUDENT
            // =====================================

            var student =
                await _studentRepository
                    .GetByIdAsync(
                        request.StudentId,
                        cancellationToken);

            if (student is null)
            {
                throw new KeyNotFoundException(
                    "Student was not found.");
            }

            if (!student.IsActive)
            {
                throw new InvalidOperationException(
                    "Payment schedule cannot be created for an inactive student.");
            }


            StudentFeeAssignment?
                feeAssignment = null;


            // =====================================
            // VALIDATE FEE ASSIGNMENT
            // =====================================

            if (request.StudentFeeAssignmentId.HasValue)
            {
                if (request.StudentFeeAssignmentId.Value ==
                    Guid.Empty)
                {
                    throw new ArgumentException(
                        "Invalid student fee assignment.");
                }


                feeAssignment =
                    await _studentFeeAssignmentRepository
                        .GetByIdAsync(
                            request.StudentFeeAssignmentId.Value,
                            cancellationToken);


                if (feeAssignment is null)
                {
                    throw new KeyNotFoundException(
                        "Student fee assignment was not found.");
                }


                // =================================
                // ASSIGNMENT MUST BELONG TO STUDENT
                // =================================

                if (feeAssignment.StudentId !=
                    request.StudentId)
                {
                    throw new InvalidOperationException(
                        "The fee assignment does not belong to this student.");
                }


                // =================================
                // ASSIGNMENT MUST BE ACTIVE
                // =================================

                if (!feeAssignment.IsActive ||
                    feeAssignment.IsDeleted)
                {
                    throw new InvalidOperationException(
                        "The student fee assignment is not active.");
                }


                // =================================
                // CHECK SCHEDULE LIMIT
                // =================================

                var existingScheduledAmount =
                    await _paymentScheduleRepository
                        .GetTotalScheduledAmountAsync(
                            feeAssignment.Id,
                            cancellationToken);


                var newTotalScheduledAmount =
                    existingScheduledAmount +
                    request.ScheduledAmount;


                if (newTotalScheduledAmount >
                    feeAssignment.NetAmount)
                {
                    var remainingAmount =
                        feeAssignment.NetAmount -
                        existingScheduledAmount;

                    throw new InvalidOperationException(
                        $"Scheduled amount exceeds the remaining assignable amount. Remaining amount: {remainingAmount:0.00}.");
                }
            }


            // =====================================
            // CREATE ENTITY
            // =====================================

            var schedule =
                new PaymentSchedule
                {
                    StudentId =
                        request.StudentId,

                    StudentFeeAssignmentId =
                        request.StudentFeeAssignmentId,

                    Title =
                        request.Title.Trim(),

                    ScheduledAmount =
                        request.ScheduledAmount,

                    DueDate =
                        request.DueDate,

                    Remarks =
                        NormalizeOptionalText(
                            request.Remarks),

                    IsActive =
                        true,

                    IsDeleted =
                        false
                };


            // =====================================
            // ADD SCHEDULE TO DB CONTEXT
            // =====================================

            await _paymentScheduleRepository
                .AddAsync(
                    schedule,
                    cancellationToken);


            // =====================================
            // FIND EXISTING UNALLOCATED PAYMENTS
            // =====================================

            var availablePayments =
                await _studentPaymentRepository
                    .GetValidPaymentsWithUnallocatedBalanceAsync(
                        student.Id,
                        cancellationToken);


            var remainingScheduleAmount =
                schedule.ScheduledAmount;


            // =====================================
            // AUTO ALLOCATE OLD ADVANCE
            // OLDEST PAYMENT FIRST
            // =====================================

            foreach (var payment in availablePayments)
            {
                if (remainingScheduleAmount <= 0)
                {
                    break;
                }


                var alreadyAllocatedFromPayment =
                    payment.Allocations
                        .Sum(x =>
                            x.AllocatedAmount);


                var availablePaymentAmount =
                    payment.Amount -
                    alreadyAllocatedFromPayment;


                if (availablePaymentAmount <= 0)
                {
                    continue;
                }


                var amountToAllocate =
                    Math.Min(
                        availablePaymentAmount,
                        remainingScheduleAmount);


                if (amountToAllocate <= 0)
                {
                    continue;
                }


                var allocation =
                    new PaymentAllocation
                    {
                        StudentPaymentId =
                            payment.Id,

                        PaymentScheduleId =
                            schedule.Id,

                        AllocatedAmount =
                            amountToAllocate,

                        Remarks =
                            "Auto allocated from existing advance payment."
                    };


                // =================================
                // ADD ALLOCATION ONCE
                //
                // Repository adds the entity to the
                // current DbContext. Do not add the
                // same entity again to the payment
                // navigation collection.
                // =================================

                await _paymentAllocationRepository
                    .AddAsync(
                        allocation,
                        cancellationToken);


                remainingScheduleAmount -=
                    amountToAllocate;
            }


            // =====================================
            // SINGLE DATABASE SAVE
            // =====================================

            await _paymentScheduleRepository
                .SaveChangesAsync(
                    cancellationToken);


            // =====================================
            // RETURN DTO
            //
            // New allocations belong to valid,
            // non-reversed payments.
            // =====================================

            var paidAmount =
                schedule.ScheduledAmount -
                remainingScheduleAmount;


            return MapToDto(
                schedule,
                student,
                feeAssignment,
                paidAmount);
        }


        // =========================================
        // GET BY ID
        // =========================================

        public async Task<PaymentScheduleDto?>
            GetByIdAsync(
                Guid id,
                CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                return null;
            }


            var schedule =
                await _paymentScheduleRepository
                    .GetByIdAsync(
                        id,
                        cancellationToken);


            if (schedule is null)
            {
                return null;
            }


            return MapToDto(
                schedule,
                schedule.Student,
                schedule.StudentFeeAssignment);
        }


        // =========================================
        // GET BY STUDENT
        // =========================================

        public async Task<IReadOnlyList<PaymentScheduleDto>>
            GetByStudentIdAsync(
                Guid studentId,
                CancellationToken cancellationToken = default)
        {
            if (studentId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Student is required.");
            }


            var schedules =
                await _paymentScheduleRepository
                    .GetByStudentIdAsync(
                        studentId,
                        cancellationToken);


            return schedules
                .Select(
                    schedule =>
                        MapToDto(
                            schedule,
                            schedule.Student,
                            schedule.StudentFeeAssignment))
                .ToList();
        }


        // =========================================
        // GET BY FEE ASSIGNMENT
        // =========================================

        public async Task<IReadOnlyList<PaymentScheduleDto>>
            GetByFeeAssignmentIdAsync(
                Guid studentFeeAssignmentId,
                CancellationToken cancellationToken = default)
        {
            if (studentFeeAssignmentId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Student fee assignment is required.");
            }


            var schedules =
                await _paymentScheduleRepository
                    .GetByFeeAssignmentIdAsync(
                        studentFeeAssignmentId,
                        cancellationToken);


            return schedules
                .Select(
                    schedule =>
                        MapToDto(
                            schedule,
                            schedule.Student,
                            schedule.StudentFeeAssignment))
                .ToList();
        }


        // =========================================
        // MAP ENTITY TO DTO
        // =========================================

        private static PaymentScheduleDto
            MapToDto(
                PaymentSchedule schedule,
                Student student,
                StudentFeeAssignment? feeAssignment,
                decimal? paidAmountOverride = null)
        {
            // =====================================
            // VALID PAID AMOUNT
            // REVERSED PAYMENTS ARE EXCLUDED
            // =====================================

            var paidAmount =
                paidAmountOverride ??
                schedule.Allocations
                    .Where(x =>
                        x.StudentPayment != null &&
                        !x.StudentPayment.IsReversed)
                    .Sum(x =>
                        x.AllocatedAmount);


            // Safety protection against inconsistent
            // historical data.

            if (paidAmount < 0)
            {
                paidAmount = 0;
            }


            var remainingAmount =
                schedule.ScheduledAmount -
                paidAmount;


            if (remainingAmount < 0)
            {
                remainingAmount = 0;
            }


            // =====================================
            // PAYMENT STATUS
            // =====================================

            string status;

            if (remainingAmount <= 0)
            {
                status = "Paid";
            }
            else if (paidAmount > 0)
            {
                status = "Partial";
            }
            else
            {
                status = "Unpaid";
            }


            // =====================================
            // DUE STATUS
            // BANGLADESH BUSINESS DATE
            // =====================================

            var today =
                GetBangladeshToday();


            var hasRemainingBalance =
                remainingAmount > 0;


            var isDueToday =
                hasRemainingBalance &&
                schedule.IsActive &&
                schedule.DueDate == today;


            var isOverdue =
                hasRemainingBalance &&
                schedule.IsActive &&
                schedule.DueDate < today;


            // =====================================
            // DTO
            // =====================================

            return new PaymentScheduleDto
            {
                Id =
                    schedule.Id,

                StudentId =
                    schedule.StudentId,

                StudentCode =
                    student.StudentCode,

                StudentName =
                    student.FullName,

                StudentFeeAssignmentId =
                    schedule.StudentFeeAssignmentId,

                FeeTypeId =
                    feeAssignment?.FeeTypeId,

                FeeTypeName =
                    feeAssignment?.FeeType?.Name,

                Title =
                    schedule.Title,

                ScheduledAmount =
                    schedule.ScheduledAmount,

                PaidAmount =
                    paidAmount,

                RemainingAmount =
                    remainingAmount,

                DueDate =
                    schedule.DueDate,

                Status =
                    status,

                IsDueToday =
                    isDueToday,

                IsOverdue =
                    isOverdue,

                Remarks =
                    schedule.Remarks,

                IsActive =
                    schedule.IsActive,

                CreatedAt =
                    schedule.CreatedAt,

                UpdatedAt =
                    schedule.UpdatedAt
            };
        }


        // =========================================
        // BANGLADESH BUSINESS DATE
        // =========================================

        private static DateOnly
            GetBangladeshToday()
        {
            TimeZoneInfo timeZone;

            try
            {
                // Windows
                timeZone =
                    TimeZoneInfo.FindSystemTimeZoneById(
                        "Bangladesh Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                // Linux fallback
                timeZone =
                    TimeZoneInfo.FindSystemTimeZoneById(
                        "Asia/Dhaka");
            }
            catch (InvalidTimeZoneException)
            {
                // Linux fallback
                timeZone =
                    TimeZoneInfo.FindSystemTimeZoneById(
                        "Asia/Dhaka");
            }


            var bangladeshNow =
                TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    timeZone);


            return DateOnly.FromDateTime(
                bangladeshNow);
        }


        // =========================================
        // NORMALIZE OPTIONAL TEXT
        // =========================================

        private static string?
            NormalizeOptionalText(
                string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }
    }
}