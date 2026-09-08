using AKLC.Domain.Entities;

namespace AKLC.Application.Interfaces
{
    public interface IAccountHeadRepository
    {
        Task<IReadOnlyList<AccountHead>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<AccountHead?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<AccountHead?> GetByIdForUpdateAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(
            string name,
            string type,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            AccountHead accountHead,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}