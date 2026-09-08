using AKLC.Domain.Entities;

namespace AKLC.Application.Interfaces
{
    public interface IPaymentScheduleRepository
    {
        Task AddAsync(
            PaymentSchedule schedule,
            CancellationToken cancellationToken = default);

        Task<PaymentSchedule?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<PaymentSchedule?> GetByIdForUpdateAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<PaymentSchedule>>
            GetByStudentIdAsync(
                Guid studentId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<PaymentSchedule>>
            GetByFeeAssignmentIdAsync(
                Guid studentFeeAssignmentId,
                CancellationToken cancellationToken = default);

        Task<decimal> GetTotalScheduledAmountAsync(
            Guid studentFeeAssignmentId,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}