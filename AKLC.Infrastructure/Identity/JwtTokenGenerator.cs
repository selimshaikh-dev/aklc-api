using AKLC.Application.Interfaces;
using AKLC.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AKLC.Infrastructure.Identity
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtSettings _settings;

        public JwtTokenGenerator(
            IOptions<JwtSettings> options)
        {
            _settings = options.Value;
        }

        public (string Token, DateTime ExpiresAt) Generate(User user)
        {
            var expiresAt =
                DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes);

            var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                user.Id.ToString()),

            new(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),

            new(
                ClaimTypes.Name,
                user.FullName),

            new(
                ClaimTypes.MobilePhone,
                user.MobileNumber),

            new(
                ClaimTypes.Role,
                user.Role.Name)
        };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_settings.Key));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            var tokenString =
                new JwtSecurityTokenHandler()
                    .WriteToken(token);

            return (tokenString, expiresAt);
        }
    }
}