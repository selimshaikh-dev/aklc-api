namespace AKLC.Api.Models.Students
{
    public class CreateStudentForm
    {
        // =========================================
        // BASIC INFORMATION
        // =========================================

        public string FullName { get; set; } = string.Empty;

        public string MobileNumber { get; set; } = string.Empty;

        public string? Email { get; set; }


        // =========================================
        // GUARDIAN / EMERGENCY CONTACT
        // =========================================

        public string? GuardianName { get; set; }

        public string? GuardianMobile { get; set; }

        public string? EmergencyMobileNumber { get; set; }


        // =========================================
        // PERSONAL INFORMATION
        // =========================================

        public DateOnly? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public string? NidNumber { get; set; }

        public string? Address { get; set; }


        // =========================================
        // EDUCATION STATUS
        // =========================================

        public bool IsBelowSsc { get; set; }


        // =========================================
        // ADMISSION INFORMATION
        // =========================================

        public DateTime AdmissionDate { get; set; }

        public bool IsActive { get; set; } = true;


        // =========================================
        // STUDENT PHOTO
        // =========================================

        public IFormFile? StudentPhoto { get; set; }


        // =========================================
        // EDUCATION QUALIFICATIONS
        // =========================================

        public List<StudentEducationForm> EducationQualifications { get; set; }
            = new();


        // =========================================
        // PROFESSIONAL EXPERIENCES
        // =========================================

        public List<StudentProfessionalExperienceForm> ProfessionalExperiences { get; set; }
            = new();


        // =========================================
        // LANGUAGE PROFICIENCIES
        // =========================================

        public List<StudentLanguageProficiencyForm> LanguageProficiencies { get; set; }
            = new();
    }
}