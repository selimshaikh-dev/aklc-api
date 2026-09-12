using AKLC.Domain.Common;

namespace AKLC.Domain.Entities
{
    public class Student : BaseEntity
    {
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

        public bool IsBelowSsc { get; set; } = false;

        public DateTime AdmissionDate { get; set; }

        public string? PhotoPath { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;


        // =========================================
        // STUDENT PROFILE RELATIONSHIPS
        // =========================================

        public ICollection<StudentEducationQualification>
            EducationQualifications
        { get; set; }
                = new List<StudentEducationQualification>();

        public ICollection<StudentProfessionalExperience>
            ProfessionalExperiences
        { get; set; }
                = new List<StudentProfessionalExperience>();

        public ICollection<StudentLanguageProficiency>
            LanguageProficiencies
        { get; set; }
                = new List<StudentLanguageProficiency>();


        // =========================================
        // FINANCIAL RELATIONSHIPS
        // =========================================

        public ICollection<StudentFeeAssignment> FeeAssignments { get; set; }
            = new List<StudentFeeAssignment>();

        public ICollection<PaymentSchedule> PaymentSchedules { get; set; }
            = new List<PaymentSchedule>();

        public ICollection<StudentPayment> Payments { get; set; }
            = new List<StudentPayment>();
    }
}