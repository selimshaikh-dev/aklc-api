using AKLC.Domain.Common;

namespace AKLC.Domain.Entities
{
    public class StudentPayment : BaseEntity
    {
        public Guid StudentId { get; set; }

        // Example: AKLC-RCP-000001
        public string ReceiptNumber { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        // Cash / Bank / bKash / Nagad / Rocket / Other
        public string PaymentMethod { get; set; } = string.Empty;

        // Transaction ID / Cheque No / Bank Reference
        public string? ReferenceNumber { get; set; }

        public string? Remarks { get; set; }

        // =========================================
        // REVERSAL
        // =========================================

        public bool IsReversed { get; set; } = false;

        public DateTime? ReversedAt { get; set; }

        public string? ReversalReason { get; set; }

        public Guid? ReversedByUserId { get; set; }

        // =========================================
        // NAVIGATION
        // =========================================

        public Student Student { get; set; } = null!;

        public ICollection<PaymentAttachment> Attachments { get; set; }
            = new List<PaymentAttachment>();

        public ICollection<PaymentAllocation> Allocations { get; set; }
            = new List<PaymentAllocation>();
    }
}