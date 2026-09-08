using AKLC.Domain.Common;

namespace AKLC.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;

        public string MobileNumber { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string PasswordHash { get; set; } = string.Empty;

        public Guid RoleId { get; set; }

        public Role Role { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        public DateTime? LastLoginAt { get; set; }
    }
}