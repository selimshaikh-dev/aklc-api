using AKLC.Application.DTOs.PaymentAllocations;
using AKLC.Application.Interfaces;
using AKLC.Domain.Entities;

namespace AKLC.Application.Services
{
    public class PaymentAllocationService
        : IPaymentAllocationService
    {
        private readonly IPaymentAllocationRepository
            _paymentAllocationRepository;

        private readonly IStudentPaymentRepository
            _studentPaymentRepository;

        private readonly IPaymentScheduleRepository
            _paymentScheduleRepository;


        // =========================================
        // CONSTRUCTOR
        // =========================================

        public PaymentAllocationService(
            IPaymentAllocationRepository paymentAllocationRepository,
            IStudentPaymentRepository studentPaymentRepository,
            IPaymentScheduleRepository paymentScheduleRepository)
        {
            _paymentAllocationRepository =
                paymentAllocationRepository;

            _studentPaymentRepository =
                studentPaymentRepository;

            _paymentScheduleRepository =
                paymentScheduleRepository;
        }


        // =========================================
        // CREATE ALLOCATION
        // =========================================

        public async Task<PaymentAllocationDto> CreateAsync(
            CreatePaymentAllocationRequest request,
            CancellationToken cancellationToken = default)
        {
            // -------------------------------------
            // BASIC VALIDATION
            // -------------------------------------

            if (request.StudentPaymentId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Student payment is required.");
            }

            if (request.PaymentScheduleId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Payment schedule is required.");
            }

            if (request.AllocatedAmount <= 0)
            {
                throw new ArgumentException(
                    "Allocated amount must be greater than zero.");
            }


            // -------------------------------------
            // GET PAYMENT
            // -------------------------------------

            var payment =
                await _studentPaymentRepository
                    .GetByIdAsync(
                        request.StudentPaymentId,
                        cancellationToken);

            if (payment is null)
            {
                throw new KeyNotFoundException(
                    "Student payment was not found.");
            }

            if (payment.IsReversed)
            {
                throw new InvalidOperationException(
                    "A reversed payment cannot be allocated.");
            }


            // -------------------------------------
            // GET SCHEDULE
            // -------------------------------------

            var schedule =
                await _paymentScheduleRepository
                    .GetByIdAsync(
                        request.PaymentScheduleId,
                        cancellationToken);

            if (schedule is null)
            {
                throw new KeyNotFoundException(
                    "Payment schedule was not found.");
            }

            if (!schedule.IsActive ||
                schedule.IsDeleted)
            {
                throw new InvalidOperationException(
                    "The payment schedule is not active.");
            }


            // -------------------------------------
            // STUDENT MUST MATCH
            // -------------------------------------

            if (payment.StudentId != schedule.StudentId)
            {
                throw new InvalidOperationException(
                    "The payment and payment schedule do not belong to the same student.");
            }


            // -------------------------------------
            // CHECK PAYMENT REMAINING BALANCE
            // -------------------------------------

            var totalAllocatedFromPayment =
                await _paymentAllocationRepository
                    .GetTotalAllocatedFromPaymentAsync(
                        payment.Id,
                        cancellationToken);

            var paymentRemainingAmount =
                payment.Amount -
                totalAllocatedFromPayment;

            if (request.AllocatedAmount >
                paymentRemainingAmount)
            {
                throw new InvalidOperationException(
                    $"Allocation exceeds the remaining payment amount. Remaining payment amount: {paymentRemainingAmount:0.00}.");
            }


            // -------------------------------------
            // CHECK SCHEDULE REMAINING BALANCE
            // -------------------------------------

            var validAllocatedToSchedule =
                await _paymentAllocationRepository
                    .GetValidAllocatedAmountAsync(
                        schedule.Id,
                        cancellationToken);

            var scheduleRemainingAmount =
                schedule.ScheduledAmount -
                validAllocatedToSchedule;

            if (request.AllocatedAmount >
                scheduleRemainingAmount)
            {
                throw new InvalidOperationException(
                    $"Allocation exceeds the remaining schedule amount. Remaining schedule amount: {scheduleRemainingAmount:0.00}.");
            }


            // -------------------------------------
            // CREATE ALLOCATION
            // -------------------------------------

            var allocation =
                new PaymentAllocation
                {
                    StudentPaymentId =
                        payment.Id,

                    PaymentScheduleId =
                        schedule.Id,

                    AllocatedAmount =
                        request.AllocatedAmount,

                    Remarks =
                        NormalizeOptionalText(
                            request.Remarks)
                };


            await _paymentAllocationRepository
                .AddAsync(
                    allocation,
                    cancellationToken);

            await _paymentAllocationRepository
                .SaveChangesAsync(
                    cancellationToken);


            // -------------------------------------
            // RETURN DTO
            // -------------------------------------

            return MapToDto(
                allocation,
                payment,
                schedule);
        }


        // =========================================
        // GET BY PAYMENT
        // =========================================

        public async Task<IReadOnlyList<PaymentAllocationDto>>
            GetByPaymentIdAsync(
                Guid studentPaymentId,
                CancellationToken cancellationToken = default)
        {
            if (studentPaymentId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Student payment is required.");
            }

            var allocations =
                await _paymentAllocationRepository
                    .GetByPaymentIdAsync(
                        studentPaymentId,
                        cancellationToken);

            return allocations
                .Select(
                    allocation =>
                        MapToDto(
                            allocation,
                            allocation.StudentPayment,
                            allocation.PaymentSchedule))
                .ToList();
        }


        // =========================================
        // GET BY SCHEDULE
        // =========================================

        public async Task<IReadOnlyList<PaymentAllocationDto>>
            GetByScheduleIdAsync(
                Guid paymentScheduleId,
                CancellationToken cancellationToken = default)
        {
            if (paymentScheduleId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Payment schedule is required.");
            }

            var allocations =
                await _paymentAllocationRepository
                    .GetByScheduleIdAsync(
                        paymentScheduleId,
                        cancellationToken);

            return allocations
                .Select(
                    allocation =>
                        MapToDto(
                            allocation,
                            allocation.StudentPayment,
                            allocation.PaymentSchedule))
                .ToList();
        }


        // =========================================
        // MAP DTO
        // =========================================

        private static PaymentAllocationDto MapToDto(
            PaymentAllocation allocation,
            StudentPayment payment,
            PaymentSchedule schedule)
        {
            return new PaymentAllocationDto
            {
                Id =
                    allocation.Id,

                StudentPaymentId =
                    allocation.StudentPaymentId,

                ReceiptNumber =
                    payment.ReceiptNumber,

                PaymentScheduleId =
                    allocation.PaymentScheduleId,

                ScheduleTitle =
                    schedule.Title,

                AllocatedAmount =
                    allocation.AllocatedAmount,

                Remarks =
                    allocation.Remarks,

                IsPaymentReversed =
                    payment.IsReversed,

                CreatedAt =
                    allocation.CreatedAt,

                UpdatedAt =
                    allocation.UpdatedAt
            };
        }


        // =========================================
        // NORMALIZE OPTIONAL TEXT
        // =========================================

        private static string? NormalizeOptionalText(
            string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }
    }
}