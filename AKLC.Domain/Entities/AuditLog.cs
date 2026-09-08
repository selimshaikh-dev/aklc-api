using AKLC.Domain.Common;

namespace AKLC.Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        public Guid? UserId { get; set; }

        // CREATE_PAYMENT
        // REVERSE_PAYMENT
        // CREATE_STUDENT
        // UPDATE_STUDENT
        // ASSIGN_FEE
        public string Action { get; set; } = string.Empty;

        public string EntityName { get; set; } = string.Empty;

        public string? EntityId { get; set; }

        public string? Description { get; set; }

        // JSON values can be stored here
        public string? OldValues { get; set; }

        public string? NewValues { get; set; }

        public string? IpAddress { get; set; }

        public string? UserAgent { get; set; }
    }
}