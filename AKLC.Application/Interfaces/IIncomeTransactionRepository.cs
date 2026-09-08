using AKLC.Domain.Entities;

namespace AKLC.Application.Interfaces
{
    public interface IIncomeTransactionRepository
    {
        Task<IReadOnlyList<IncomeTransaction>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<IncomeTransaction?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            IncomeTransaction transaction,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsByVoucherNumberAsync(
            string voucherNumber,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}