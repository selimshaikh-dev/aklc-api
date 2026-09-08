namespace AKLC.Application.DTOs.Students
{
    public sealed class StudentSummaryDto
    {
        public int TotalStudents { get; set; }

        public int ActiveStudents { get; set; }

        public int DueStudentCount { get; set; }

        public decimal TotalOutstanding { get; set; }
    }
}