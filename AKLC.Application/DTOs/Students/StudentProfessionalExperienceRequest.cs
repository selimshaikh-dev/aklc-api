namespace AKLC.Application.DTOs.Students
{
    public class StudentProfessionalExperienceRequest
    {
        public string ExperienceTitle { get; set; } = string.Empty;

        public string? OrganizationName { get; set; }

        public decimal? YearsOfExperience { get; set; }

        public string? Remarks { get; set; }
    }
}