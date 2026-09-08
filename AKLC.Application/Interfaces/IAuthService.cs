using AKLC.Application.DTOs.Auth;

namespace AKLC.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(
            LoginRequest request,
            CancellationToken cancellationToken = default);
    }
}