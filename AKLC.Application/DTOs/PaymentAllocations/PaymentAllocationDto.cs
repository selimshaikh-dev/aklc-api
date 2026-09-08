namespace AKLC.Application.DTOs.PaymentAllocations
{
    public class PaymentAllocationDto
    {
        public Guid Id { get; set; }

        public Guid StudentPaymentId { get; set; }

        public string ReceiptNumber { get; set; } = string.Empty;

        public Guid PaymentScheduleId { get; set; }

        public string ScheduleTitle { get; set; } = string.Empty;

        public decimal AllocatedAmount { get; set; }

        public string? Remarks { get; set; }

        public bool IsPaymentReversed { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}