using AKLC.Domain.Entities;

namespace AKLC.Application.Interfaces
{
    public interface IPaymentAllocationRepository
    {
        Task AddAsync(
            PaymentAllocation allocation,
            CancellationToken cancellationToken = default);

        Task AddRangeAsync(
            IEnumerable<PaymentAllocation> allocations,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<PaymentAllocation>>
            GetByPaymentIdAsync(
                Guid studentPaymentId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<PaymentAllocation>>
            GetByScheduleIdAsync(
                Guid paymentScheduleId,
                CancellationToken cancellationToken = default);

        Task<decimal> GetValidAllocatedAmountAsync(
            Guid paymentScheduleId,
            CancellationToken cancellationToken = default);

        Task<decimal> GetTotalAllocatedFromPaymentAsync(
            Guid studentPaymentId,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}