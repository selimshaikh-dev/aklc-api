namespace AKLC.Application.DTOs.Students
{
    public class StudentFinancialSummaryDto
    {
        public Guid StudentId { get; set; }

        public string StudentCode { get; set; } =
            string.Empty;

        public string StudentName { get; set; } =
            string.Empty;


        // =========================================
        // FINANCIAL SUMMARY
        // =========================================

        public decimal TotalPayable { get; set; }

        public decimal TotalPaid { get; set; }

        public decimal OutstandingAmount { get; set; }

        public decimal AdvanceAmount { get; set; }


        // =========================================
        // PAYMENT STATUS
        // =========================================

        public string PaymentStatus { get; set; } =
            string.Empty;


        // =========================================
        // NEXT PAYMENT
        // =========================================

        public DateOnly? NextPaymentDate { get; set; }

        public decimal NextPaymentAmount { get; set; }


        // =========================================
        // DUE FLAGS
        // =========================================

        public bool IsDueToday { get; set; }

        public bool IsOverdue { get; set; }
    }
}