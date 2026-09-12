namespace AKLC.Application.DTOs.StudentPayments
{
    public class CreateStudentPaymentRequest
    {
        public Guid StudentId { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string PaymentMethod { get; set; }
            = string.Empty;

        public string? ReferenceNumber { get; set; }

        public string? Remarks { get; set; }


        // =========================================
        // NEXT PAYMENT
        // =========================================

        public DateOnly? NextPaymentDueDate { get; set; }


        // =========================================
        // PAYMENT PROOF ATTACHMENTS
        // =========================================

        public IReadOnlyList<CreatePaymentAttachmentRequest>
            Attachments
        { get; set; }
            = new List<CreatePaymentAttachmentRequest>();
    }
}