namespace AKLC.Application.DTOs.StudentFeeAssignments
{
    public class CreateStudentFeeAssignmentRequest
    {
        public Guid StudentId { get; set; }

        public Guid FeeTypeId { get; set; }

        public decimal Amount { get; set; }

        public decimal DiscountAmount { get; set; }

        public DateOnly? DueDate { get; set; }

        public string? Remarks { get; set; }
    }
}