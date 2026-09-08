using AKLC.Domain.Entities;

namespace AKLC.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        (string Token, DateTime ExpiresAt) Generate(User user);
    }
}