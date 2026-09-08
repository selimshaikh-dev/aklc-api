namespace AKLC.Application.DTOs.Students
{
    public sealed class DueStudentDto
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

        public decimal DueAmount { get; set; }


        // =========================================
        // PAYMENT SCHEDULE
        // =========================================

        public DateOnly? NextPaymentDate { get; set; }

        public decimal NextPaymentAmount { get; set; }


        // =========================================
        // DUE STATUS
        // =========================================

        public bool IsDueToday { get; set; }

        public bool IsOverdue { get; set; }
    }
}