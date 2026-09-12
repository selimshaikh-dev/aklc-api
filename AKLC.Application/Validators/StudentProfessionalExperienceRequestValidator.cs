using AKLC.Application.DTOs.Students;
using FluentValidation;

namespace AKLC.Application.Validators
{
    public class StudentProfessionalExperienceRequestValidator
        : AbstractValidator<StudentProfessionalExperienceRequest>
    {
        public StudentProfessionalExperienceRequestValidator()
        {
            // =========================================
            // EXPERIENCE TITLE
            // =========================================

            RuleFor(x => x.ExperienceTitle)
                .NotEmpty()
                .WithMessage("Experience title is required.")
                .MaximumLength(150)
                .WithMessage("Experience title cannot exceed 150 characters.");


            // =========================================
            // ORGANIZATION NAME
            // =========================================

            RuleFor(x => x.OrganizationName)
                .MaximumLength(200)
                .WithMessage("Organization name cannot exceed 200 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.OrganizationName));


            // =========================================
            // YEARS OF EXPERIENCE
            // =========================================

            RuleFor(x => x.YearsOfExperience)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Years of experience cannot be negative.")
                .LessThanOrEqualTo(100)
                .WithMessage("Years of experience cannot exceed 100 years.")
                .When(x => x.YearsOfExperience.HasValue);


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