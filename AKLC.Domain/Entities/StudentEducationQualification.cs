using AKLC.Domain.Common;
using AKLC.Domain.Enums;

namespace AKLC.Domain.Entities
{
    public class StudentEducationQualification : BaseEntity
    {
        public Guid StudentId { get; set; }

        public EducationLevel EducationLevel { get; set; }

        public string? ExaminationName { get; set; }

        public int? PassingYear { get; set; }

        public string? Result { get; set; }

        public string? BoardOrUniversity { get; set; }

        public string? InstitutionName { get; set; }

        public string? Remarks { get; set; }

        public int DisplayOrder { get; set; }

        public Student Student { get; set; } = null!;
    }
}