using AKLC.Application.Interfaces;
using AKLC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AKLC.Infrastructure.Persistence.Seed
{
    public class AdminSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IConfiguration _configuration;

        public AdminSeeder(
            ApplicationDbContext context,
            IPasswordHasher passwordHasher,
            IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public async Task SeedAsync()
        {
            var mobileNumber =
                _configuration["InitialAdmin:MobileNumber"];

            var password =
                _configuration["InitialAdmin:Password"];

            var fullName =
                _configuration["InitialAdmin:FullName"];

            if (string.IsNullOrWhiteSpace(mobileNumber) ||
                string.IsNullOrWhiteSpace(password))
            {
                return;
            }

            // Check if admin already exists
            var adminExists = await _context.Users
                .AsNoTracking()
                .AnyAsync(user =>
                    user.MobileNumber == mobileNumber);

            if (adminExists)
            {
                return;
            }

            var admin = new User
            {
                Id = Guid.NewGuid(),

                FullName =
                    string.IsNullOrWhiteSpace(fullName)
                        ? "Administrator"
                        : fullName,

                MobileNumber = mobileNumber,

                PasswordHash =
                    _passwordHasher.Hash(password),

                RoleId = SeedData.AdminRoleId,

                IsActive = true,

                IsDeleted = false,

                CreatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(admin);

            await _context.SaveChangesAsync();
        }
    }
}