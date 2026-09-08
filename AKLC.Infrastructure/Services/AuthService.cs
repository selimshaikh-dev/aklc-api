using AKLC.Application.DTOs.Auth;
using AKLC.Application.Interfaces;
using AKLC.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AKLC.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(
            ApplicationDbContext context,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<LoginResponse?> LoginAsync(
            LoginRequest request,
            CancellationToken cancellationToken = default)
        {
            var user = await _context.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(
                    x =>
                        x.MobileNumber == request.MobileNumber &&
                        x.IsActive &&
                        !x.IsDeleted,
                    cancellationToken);

            if (user is null)
            {
                return null;
            }

            var passwordValid =
                _passwordHasher.Verify(
                    request.Password,
                    user.PasswordHash);

            if (!passwordValid)
            {
                return null;
            }

            user.LastLoginAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            var token =
                _jwtTokenGenerator.Generate(user);

            return new LoginResponse
            {
                AccessToken = token.Token,

                ExpiresAt = token.ExpiresAt,

                User = new UserInfoDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    MobileNumber = user.MobileNumber,
                    Role = user.Role.Name
                }
            };
        }
    }
}