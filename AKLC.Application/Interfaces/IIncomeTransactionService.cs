using AKLC.Application.DTOs.Accounts;

namespace AKLC.Application.Interfaces
{
    public interface IIncomeTransactionService
    {
        Task<IReadOnlyList<IncomeTransactionDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<IncomeTransactionDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<IncomeTransactionDto> CreateAsync(
            CreateIncomeTransactionRequest request,
            CancellationToken cancellationToken = default);
    }
}