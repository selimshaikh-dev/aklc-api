namespace AKLC.Api.Models.Students
{
    public class StudentLanguageProficiencyForm
    {
        public string LanguageName { get; set; } = string.Empty;

        public string? ProficiencyLevel { get; set; }

        public string? InstitutionName { get; set; }

        public string? Remarks { get; set; }
    }
}