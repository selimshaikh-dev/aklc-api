namespace AKLC.Application.DTOs.Students
{
    public class StudentDto
    {
        public Guid Id { get; set; }

        public string StudentCode { get; set; } = string.Empty;

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

        public bool IsActive { get; set; }

        public string? PhotoPath { get; set; }


        // =========================================
        // EDUCATION QUALIFICATIONS
        // =========================================

        public List<StudentEducationDto> EducationQualifications { get; set; }
            = new();


        // =========================================
        // PROFESSIONAL EXPERIENCES
        // =========================================

        public List<StudentProfessionalExperienceDto> ProfessionalExperiences { get; set; }
            = new();


        // =========================================
        // LANGUAGE PROFICIENCIES
        // =========================================

        public List<StudentLanguageProficiencyDto> LanguageProficiencies { get; set; }
            = new();
    }
}