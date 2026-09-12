namespace AKLC.Api.Models.Students
{
    public class StudentProfessionalExperienceForm
    {
        public string ExperienceTitle { get; set; } = string.Empty;

        public string? OrganizationName { get; set; }

        public decimal? YearsOfExperience { get; set; }

        public string? Remarks { get; set; }
    }
}