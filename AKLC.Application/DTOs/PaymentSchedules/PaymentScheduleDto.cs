namespace AKLC.Application.DTOs.PaymentSchedules
{
    public class PaymentScheduleDto
    {
        public Guid Id { get; set; }

        public Guid StudentId { get; set; }

        public string StudentCode { get; set; } =
            string.Empty;

        public string StudentName { get; set; } =
            string.Empty;

        public Guid? StudentFeeAssignmentId { get; set; }

        public Guid? FeeTypeId { get; set; }

        public string? FeeTypeName { get; set; }

        public string Title { get; set; } =
            string.Empty;


        // =========================================
        // FINANCIAL AMOUNTS
        // =========================================

        public decimal ScheduledAmount { get; set; }

        public decimal PaidAmount { get; set; }

        public decimal RemainingAmount { get; set; }


        // =========================================
        // DUE INFORMATION
        // =========================================

        public DateOnly DueDate { get; set; }

        public string Status { get; set; } =
            string.Empty;

        public bool IsDueToday { get; set; }

        public bool IsOverdue { get; set; }


        // =========================================
        // OTHER INFORMATION
        // =========================================

        public string? Remarks { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}