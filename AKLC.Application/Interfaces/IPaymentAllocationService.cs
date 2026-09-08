using AKLC.Application.DTOs.PaymentAllocations;

namespace AKLC.Application.Interfaces
{
    public interface IPaymentAllocationService
    {
        Task<PaymentAllocationDto> CreateAsync(
            CreatePaymentAllocationRequest request,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<PaymentAllocationDto>>
            GetByPaymentIdAsync(
                Guid studentPaymentId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<PaymentAllocationDto>>
            GetByScheduleIdAsync(
                Guid paymentScheduleId,
                CancellationToken cancellationToken = default);
    }
}