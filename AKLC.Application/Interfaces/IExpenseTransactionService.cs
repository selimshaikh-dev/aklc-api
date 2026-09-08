using AKLC.Application.DTOs.Accounts;

namespace AKLC.Application.Interfaces
{
    public interface IExpenseTransactionService
    {
        Task<IReadOnlyList<ExpenseTransactionDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<ExpenseTransactionDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<ExpenseTransactionDto> CreateAsync(
            CreateExpenseTransactionRequest request,
            CancellationToken cancellationToken = default);
    }
}