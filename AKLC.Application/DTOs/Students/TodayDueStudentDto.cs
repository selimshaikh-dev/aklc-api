namespace AKLC.Application.DTOs.Students
{
    public sealed class TodayDueStudentDto
    {
        public Guid Id { get; set; }

        public string StudentCode { get; set; }
            = string.Empty;

        public string FullName { get; set; }
            = string.Empty;

        public string MobileNumber { get; set; }
            = string.Empty;

        public decimal DueAmount { get; set; }

        public DateOnly NextPaymentDate { get; set; }
    }
}