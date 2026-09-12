using AKLC.Application.DTOs.Students;
using AKLC.Domain.Enums;
using FluentValidation;

namespace AKLC.Application.Validators
{
    public class CreateStudentRequestValidator
        : AbstractValidator<CreateStudentRequest>
    {
        public CreateStudentRequestValidator()
        {
            // =========================================
            // FULL NAME
            // =========================================

            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("Student name is required.")
                .MaximumLength(150)
                .WithMessage("Student name cannot exceed 150 characters.");


            // =========================================
            // MOBILE NUMBER
            // =========================================

            RuleFor(x => x.MobileNumber)
                .NotEmpty()
                .WithMessage("Mobile number is required.")
                .MaximumLength(20)
                .WithMessage("Mobile number cannot exceed 20 characters.")
                .Matches(@"^(?:\+8801|01)[3-9]\d{8}$")
                .WithMessage(
                    "Please enter a valid Bangladeshi mobile number, e.g. 01712345678 or +8801712345678.");


            // =========================================
            // EMAIL
            // =========================================

            RuleFor(x => x.Email)
                .MaximumLength(150)
                .WithMessage("Email cannot exceed 150 characters.")
                .EmailAddress()
                .WithMessage("Please enter a valid email address.")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));


            // =========================================
            // GUARDIAN NAME
            // =========================================

            RuleFor(x => x.GuardianName)
                .MaximumLength(150)
                .WithMessage("Guardian name cannot exceed 150 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.GuardianName));


            // =========================================
            // GUARDIAN MOBILE
            // =========================================

            RuleFor(x => x.GuardianMobile)
                .MaximumLength(20)
                .WithMessage("Guardian mobile number cannot exceed 20 characters.")
                .Matches(@"^(?:\+8801|01)[3-9]\d{8}$")
                .WithMessage(
                    "Please enter a valid guardian mobile number.")
                .When(x => !string.IsNullOrWhiteSpace(x.GuardianMobile));


            // =========================================
            // EMERGENCY MOBILE
            // =========================================

            RuleFor(x => x.EmergencyMobileNumber)
                .MaximumLength(20)
                .WithMessage("Emergency mobile number cannot exceed 20 characters.")
                .Matches(@"^(?:\+8801|01)[3-9]\d{8}$")
                .WithMessage(
                    "Please enter a valid emergency mobile number.")
                .When(x => !string.IsNullOrWhiteSpace(x.EmergencyMobileNumber));


            // =========================================
            // NID
            // =========================================

            RuleFor(x => x.NidNumber)
                .MaximumLength(25)
                .WithMessage("NID number cannot exceed 25 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.NidNumber));


            // =========================================
            // GENDER
            // =========================================

            RuleFor(x => x.Gender)
                .MaximumLength(20)
                .WithMessage("Gender cannot exceed 20 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Gender));


            // =========================================
            // ADDRESS
            // =========================================

            RuleFor(x => x.Address)
                .MaximumLength(500)
                .WithMessage("Address cannot exceed 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Address));


            // =========================================
            // DATE OF BIRTH
            // =========================================

            RuleFor(x => x.DateOfBirth)
                .Must(date =>
                    !date.HasValue ||
                    date.Value <= DateOnly.FromDateTime(DateTime.Today))
                .WithMessage(
                    "Date of birth cannot be in the future.");


            // =========================================
            // ADMISSION DATE
            // =========================================

            RuleFor(x => x.AdmissionDate)
                .NotEmpty()
                .WithMessage("Admission date is required.");


            // =========================================
            // PHOTO PATH
            // =========================================

            RuleFor(x => x.PhotoPath)
                .MaximumLength(500)
                .WithMessage("Photo path cannot exceed 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.PhotoPath));


            // =========================================
            // CHILD COLLECTION VALIDATION
            // =========================================

            RuleForEach(x => x.EducationQualifications)
                .SetValidator(
                    new StudentEducationRequestValidator());

            RuleForEach(x => x.ProfessionalExperiences)
                .SetValidator(
                    new StudentProfessionalExperienceRequestValidator());

            RuleForEach(x => x.LanguageProficiencies)
                .SetValidator(
                    new StudentLanguageProficiencyRequestValidator());


            // =========================================
            // BELOW SSC BUSINESS RULE
            // =========================================

            RuleFor(x => x.EducationQualifications)
                .Empty()
                .When(x => x.IsBelowSsc)
                .WithMessage(
                    "Education qualifications must be empty when the student is marked as Below SSC.");


            // =========================================
            // SSC REQUIRED BUSINESS RULE
            // =========================================

            RuleFor(x => x.EducationQualifications)
                .Must(items =>
                    items != null &&
                    items.Any(x =>
                        x.EducationLevel ==
                        EducationLevel.SscEquivalent))
                .When(x => !x.IsBelowSsc)
                .WithMessage(
                    "SSC / Equivalent qualification is required when the student is not marked as Below SSC.");


            // =========================================
            // DUPLICATE EDUCATION LEVEL
            // =========================================

            RuleFor(x => x.EducationQualifications)
                .Must(items =>
                    items == null ||
                    items
                        .GroupBy(x => x.EducationLevel)
                        .All(group => group.Count() == 1))
                .When(x =>
                    !x.IsBelowSsc &&
                    x.EducationQualifications != null)
                .WithMessage(
                    "Duplicate education levels are not allowed.");


            // =========================================
            // PROFESSIONAL EXPERIENCE LIMIT
            // =========================================

            RuleFor(x => x.ProfessionalExperiences)
                .Must(items =>
                    items == null || items.Count <= 20)
                .WithMessage(
                    "A maximum of 20 professional experience records is allowed.");


            // =========================================
            // LANGUAGE PROFICIENCY LIMIT
            // =========================================

            RuleFor(x => x.LanguageProficiencies)
                .Must(items =>
                    items == null || items.Count <= 20)
                .WithMessage(
                    "A maximum of 20 language proficiency records is allowed.");
        }
    }
}