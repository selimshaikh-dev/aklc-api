namespace AKLC.Application.DTOs.StudentFeeAssignments
{
    public class StudentFeeAssignmentDto
    {
        public Guid Id { get; set; }

        public Guid StudentId { get; set; }

        public string StudentCode { get; set; } =
            string.Empty;

        public string StudentName { get; set; } =
            string.Empty;

        public Guid FeeTypeId { get; set; }

        public string FeeTypeName { get; set; } =
            string.Empty;

        public decimal Amount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal NetAmount { get; set; }

        public DateOnly? DueDate { get; set; }

        public string? Remarks { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}