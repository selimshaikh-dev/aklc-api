namespace AKLC.Application.DTOs.Students
{
    public class CreateStudentRequest
    {
        public string FullName { get; set; } = string.Empty;

        public string MobileNumber { get; set; } = string.Empty;

        public string? GuardianName { get; set; }

        public string? GuardianMobile { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public DateTime AdmissionDate { get; set; }

        public Guid CourseId { get; set; }

        public Guid BatchId { get; set; }

        public string? PhotoPath { get; set; }
    }
}