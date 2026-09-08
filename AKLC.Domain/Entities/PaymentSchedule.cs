using AKLC.Domain.Common;

namespace AKLC.Domain.Entities
{
    public class PaymentSchedule : BaseEntity
    {
        public Guid StudentId { get; set; }

        // Nullable because schedule may represent
        // overall student balance
        public Guid? StudentFeeAssignmentId { get; set; }

        public string Title { get; set; } = string.Empty;

        public decimal ScheduledAmount { get; set; }

        public DateOnly DueDate { get; set; }

        public string? Remarks { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        // Navigation
        public Student Student { get; set; } = null!;

        public StudentFeeAssignment? StudentFeeAssignment { get; set; }

        public ICollection<PaymentAllocation> Allocations { get; set; }
            = new List<PaymentAllocation>();
    }
}