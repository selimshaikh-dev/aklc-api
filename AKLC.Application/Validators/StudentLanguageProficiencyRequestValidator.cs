using AKLC.Application.DTOs.Students;
using FluentValidation;

namespace AKLC.Application.Validators
{
    public class StudentLanguageProficiencyRequestValidator
        : AbstractValidator<StudentLanguageProficiencyRequest>
    {
        public StudentLanguageProficiencyRequestValidator()
        {
            // =========================================
            // LANGUAGE NAME
            // =========================================

            RuleFor(x => x.LanguageName)
                .NotEmpty()
                .WithMessage("Language name is required.")
                .MaximumLength(100)
                .WithMessage("Language name cannot exceed 100 characters.");


            // =========================================
            // PROFICIENCY LEVEL
            // =========================================

            RuleFor(x => x.ProficiencyLevel)
                .MaximumLength(50)
                .WithMessage("Proficiency level cannot exceed 50 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.ProficiencyLevel));


            // =========================================
            // INSTITUTION NAME
            // =========================================

            RuleFor(x => x.InstitutionName)
                .MaximumLength(200)
                .WithMessage("Institution name cannot exceed 200 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.InstitutionName));


            // =========================================
            // REMARKS
            // =========================================

            RuleFor(x => x.Remarks)
                .MaximumLength(500)
                .WithMessage("Remarks cannot exceed 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Remarks));
        }
    }
}