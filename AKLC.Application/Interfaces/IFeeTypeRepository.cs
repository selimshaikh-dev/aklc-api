using AKLC.Domain.Entities;

namespace AKLC.Application.Interfaces
{
    public interface IFeeTypeRepository
    {
        Task<FeeType?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}