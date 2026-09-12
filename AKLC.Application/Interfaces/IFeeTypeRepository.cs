using AKLC.Domain.Entities;

namespace AKLC.Application.Interfaces
{
    public interface IFeeTypeRepository
    {
        Task<IReadOnlyList<FeeType>> GetAllActiveAsync(
            CancellationToken cancellationToken = default);

        Task<FeeType?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsByNameAsync(
            string name,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            FeeType feeType,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}