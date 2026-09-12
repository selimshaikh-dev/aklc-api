namespace AKLC.Application.DTOs.Students
{
    public class StudentLanguageProficiencyDto
    {
        public Guid Id { get; set; }

        public string LanguageName { get; set; } = string.Empty;

        public string? ProficiencyLevel { get; set; }

        public string? InstitutionName { get; set; }

        public string? Remarks { get; set; }
    }
}