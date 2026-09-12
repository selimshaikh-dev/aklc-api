namespace AKLC.Api.Models.StudentPayments
{
    public class CreateStudentPaymentForm
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


        // Multiple payment proof files:
        // JPG, JPEG, PNG, WEBP, PDF
        public List<IFormFile> Attachments { get; set; }
            = new();
    }
}