namespace AKLC.Application.DTOs.StudentPayments
{
    public class StudentPaymentDto
    {
        public Guid Id { get; set; }

        public Guid StudentId { get; set; }

        public string ReceiptNumber { get; set; }
            = string.Empty;

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string PaymentMethod { get; set; }
            = string.Empty;

        public string? ReferenceNumber { get; set; }

        public string? Remarks { get; set; }

        public bool IsReversed { get; set; }

        public DateTime? ReversedAt { get; set; }

        public string? ReversalReason { get; set; }

        public DateTime CreatedAt { get; set; }


        // =========================================
        // PAYMENT PROOF ATTACHMENTS
        // =========================================

        public IReadOnlyList<PaymentAttachmentDto>
            Attachments
        { get; set; }
            = new List<PaymentAttachmentDto>();
    }
}