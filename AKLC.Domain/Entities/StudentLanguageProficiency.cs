using AKLC.Domain.Common;

namespace AKLC.Domain.Entities
{
    public class StudentLanguageProficiency : BaseEntity
    {
        public Guid StudentId { get; set; }

        public string LanguageName { get; set; } = string.Empty;

        public string? ProficiencyLevel { get; set; }

        public string? InstitutionName { get; set; }

        public string? Remarks { get; set; }

        public Student Student { get; set; } = null!;
    }
}