namespace AKLC.Application.DTOs.Students
{
    public class StudentDto
    {
        public Guid Id { get; set; }

        public string StudentCode { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string MobileNumber { get; set; } = string.Empty;

        public string? GuardianName { get; set; }

        public string? GuardianMobile { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public DateTime AdmissionDate { get; set; }

        // =========================================
        // COURSE / BATCH
        // =========================================

        public Guid CourseId { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public Guid BatchId { get; set; }

        public string BatchName { get; set; } = string.Empty;

        // =========================================
        // STUDENT STATUS
        // =========================================

        public bool IsActive { get; set; }

        public string? PhotoPath { get; set; }
    }
}