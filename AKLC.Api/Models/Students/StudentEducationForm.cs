using AKLC.Domain.Enums;

namespace AKLC.Api.Models.Students
{
    public class StudentEducationForm
    {
        public EducationLevel EducationLevel { get; set; }

        public string? ExaminationName { get; set; }

        public int? PassingYear { get; set; }

        public string? Result { get; set; }

        public string? BoardOrUniversity { get; set; }

        public string? InstitutionName { get; set; }

        public string? Remarks { get; set; }

        public int DisplayOrder { get; set; }
    }
}