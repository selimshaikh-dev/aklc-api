using AKLC.Domain.Entities;

namespace AKLC.Application.Interfaces
{
    public interface IExpenseTransactionRepository
    {
        Task<IReadOnlyList<ExpenseTransaction>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<ExpenseTransaction?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            ExpenseTransaction transaction,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsByVoucherNumberAsync(
            string voucherNumber,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}