namespace AKLC.Application.DTOs.PaymentSchedules
{
    public class CreatePaymentScheduleRequest
    {
        public Guid StudentId { get; set; }

        public Guid? StudentFeeAssignmentId { get; set; }

        public string Title { get; set; } =
            string.Empty;

        public decimal ScheduledAmount { get; set; }

        public DateOnly DueDate { get; set; }

        public string? Remarks { get; set; }
    }
}