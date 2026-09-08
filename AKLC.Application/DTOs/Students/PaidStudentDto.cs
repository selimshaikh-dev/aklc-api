namespace AKLC.Application.DTOs.Students
{
    public sealed class PaidStudentDto
    {
        // =========================================
        // STUDENT
        // =========================================

        public Guid Id { get; set; }

        public string StudentCode { get; set; }
            = string.Empty;

        public string FullName { get; set; }
            = string.Empty;

        public string MobileNumber { get; set; }
            = string.Empty;


        // =========================================
        // FINANCIAL
        // =========================================

        public decimal TotalPayable { get; set; }

        public decimal TotalPaid { get; set; }

        public decimal PaidAmount { get; set; }


        // =========================================
        // PAYMENT STATUS
        // =========================================

        public string PaymentStatus { get; set; }
            = string.Empty;

        public DateTime? LastPaymentDate { get; set; }
    }
}