using AKLC.Domain.Common;

namespace AKLC.Domain.Entities
{
    public class StudentFeeAssignment : BaseEntity
    {
        public Guid StudentId { get; set; }

        public Guid FeeTypeId { get; set; }

        // Original amount
        public decimal Amount { get; set; }

        // Discount / Waiver
        public decimal DiscountAmount { get; set; } = 0;

        // Amount - DiscountAmount
        public decimal NetAmount { get; set; }

        public DateOnly? DueDate { get; set; }

        public string? Remarks { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        // Navigation
        public Student Student { get; set; } = null!;

        public FeeType FeeType { get; set; } = null!;

        public ICollection<PaymentSchedule> PaymentSchedules { get; set; }
            = new List<PaymentSchedule>();
    }
}