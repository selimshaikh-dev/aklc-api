using AKLC.Application.DTOs.Students;
using FluentValidation;

namespace AKLC.Application.Validators
{
    public class StudentEducationRequestValidator
        : AbstractValidator<StudentEducationRequest>
    {
        public StudentEducationRequestValidator()
        {
            // =========================================
            // EDUCATION LEVEL
            // =========================================

            RuleFor(x => x.EducationLevel)
                .IsInEnum()
                .WithMessage("Please select a valid education level.");


            // =========================================
            // EXAMINATION / DEGREE NAME
            // =========================================

            RuleFor(x => x.ExaminationName)
                .MaximumLength(150)
                .WithMessage("Examination name cannot exceed 150 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.ExaminationName));


            // =========================================
            // PASSING YEAR
            // =========================================

            RuleFor(x => x.PassingYear)
                .InclusiveBetween(1950, DateTime.Today.Year)
                .WithMessage(
                    $"Passing year must be between 1950 and {DateTime.Today.Year}.")
                .When(x => x.PassingYear.HasValue);


            // =========================================
            // RESULT
            // =========================================

            RuleFor(x => x.Result)
                .MaximumLength(50)
                .WithMessage("Result cannot exceed 50 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Result));


            // =========================================
            // BOARD / UNIVERSITY
            // =========================================

            RuleFor(x => x.BoardOrUniversity)
                .MaximumLength(200)
                .WithMessage("Board or university cannot exceed 200 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.BoardOrUniversity));


            // =========================================
            // INSTITUTION
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


            // =========================================
            // DISPLAY ORDER
            // =========================================

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Display order cannot be negative.");
        }
    }
}