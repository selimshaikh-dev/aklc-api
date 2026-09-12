namespace AKLC.Application.DTOs.Students
{
    public class CreateStudentRequest
    {
        // =========================================
        // BASIC INFORMATION
        // =========================================

        public string FullName { get; set; } = string.Empty;

        public string MobileNumber { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? GuardianName { get; set; }

        public string? GuardianMobile { get; set; }

        public string? EmergencyMobileNumber { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public string? NidNumber { get; set; }

        public string? Address { get; set; }

        public bool IsBelowSsc { get; set; }

        public DateTime AdmissionDate { get; set; }

        public string? PhotoPath { get; set; }

        public bool IsActive { get; set; } = true;


        // =========================================
        // EDUCATION
        // =========================================

        public List<StudentEducationRequest> EducationQualifications { get; set; }
            = new();


        // =========================================
        // PROFESSIONAL EXPERIENCE
        // =========================================

        public List<StudentProfessionalExperienceRequest> ProfessionalExperiences { get; set; }
            = new();


        // =========================================
        // LANGUAGE PROFICIENCY
        // =========================================

        public List<StudentLanguageProficiencyRequest> LanguageProficiencies { get; set; }
            = new();
    }
}