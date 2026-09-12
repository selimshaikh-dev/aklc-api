using AKLC.Domain.Common;

namespace AKLC.Domain.Entities
{
    public class StudentProfessionalExperience : BaseEntity
    {
        public Guid StudentId { get; set; }

        public string ExperienceTitle { get; set; } = string.Empty;

        public string? OrganizationName { get; set; }

        public decimal? YearsOfExperience { get; set; }

        public string? Remarks { get; set; }

        public Student Student { get; set; } = null!;
    }
}