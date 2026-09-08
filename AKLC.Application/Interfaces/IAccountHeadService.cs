using AKLC.Application.DTOs.Accounts;

namespace AKLC.Application.Interfaces
{
    public interface IAccountHeadService
    {
        Task<IReadOnlyList<AccountHeadDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<AccountHeadDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<AccountHeadDto> CreateAsync(
            CreateAccountHeadRequest request,
            CancellationToken cancellationToken = default);
    }
}