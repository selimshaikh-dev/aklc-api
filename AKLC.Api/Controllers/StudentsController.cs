using AKLC.Api.Models.Students;
using AKLC.Application.DTOs.Students;
using AKLC.Application.Interfaces;
using AKLC.Domain.Constants;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AKLC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Admin)]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService
            _studentService;

        private readonly IFileStorageService
            _fileStorageService;

        private readonly IValidator<CreateStudentRequest>
            _createStudentRequestValidator;


        // =========================================
        // CONSTRUCTOR
        // =========================================

        public StudentsController(
            IStudentService studentService,
            IFileStorageService fileStorageService,
            IValidator<CreateStudentRequest> createStudentRequestValidator)
        {
            _studentService =
                studentService;

            _fileStorageService =
                fileStorageService;

            _createStudentRequestValidator =
                createStudentRequestValidator;
        }


        // =========================================
        // GET ALL STUDENTS
        // GET: api/Students
        // =========================================

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<StudentDto>>>
            GetAll(
                CancellationToken cancellationToken)
        {
            var students =
                await _studentService
                    .GetAllAsync(
                        cancellationToken);

            return Ok(
                students);
        }


        // =========================================
        // GET STUDENT BY ID
        // GET: api/Students/{id}
        // =========================================

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<StudentDto>>
            GetById(
                Guid id,
                CancellationToken cancellationToken)
        {
            var student =
                await _studentService
                    .GetByIdAsync(
                        id,
                        cancellationToken);

            if (student is null)
            {
                return NotFound(
                    new
                    {
                        message =
                            "Student not found."
                    });
            }

            return Ok(
                student);
        }


        // =========================================
        // STUDENT FINANCIAL SUMMARY
        // GET: api/Students/{id}/financial-summary
        // =========================================

        [HttpGet("{id:guid}/financial-summary")]
        public async Task<ActionResult<StudentFinancialSummaryDto>>
            GetFinancialSummary(
                Guid id,
                CancellationToken cancellationToken)
        {
            var summary =
                await _studentService
                    .GetFinancialSummaryAsync(
                        id,
                        cancellationToken);

            if (summary is null)
            {
                return NotFound(
                    new
                    {
                        message =
                            "Student not found."
                    });
            }

            return Ok(
                summary);
        }


        // =========================================
        // CREATE STUDENT
        // POST: api/Students
        // multipart/form-data
        // =========================================

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<StudentDto>>
            Create(
                [FromForm] CreateStudentForm form,
                CancellationToken cancellationToken)
        {
            string? photoPath = null;

            try
            {
                // =================================
                // APPLICATION REQUEST
                // =================================

                var request =
                    new CreateStudentRequest
                    {
                        // -------------------------
                        // BASIC INFORMATION
                        // -------------------------

                        FullName =
                            form.FullName,

                        MobileNumber =
                            form.MobileNumber,

                        Email =
                            form.Email,


                        // -------------------------
                        // GUARDIAN / CONTACT
                        // -------------------------

                        GuardianName =
                            form.GuardianName,

                        GuardianMobile =
                            form.GuardianMobile,

                        EmergencyMobileNumber =
                            form.EmergencyMobileNumber,


                        // -------------------------
                        // PERSONAL INFORMATION
                        // -------------------------

                        DateOfBirth =
                            form.DateOfBirth,

                        Gender =
                            form.Gender,

                        NidNumber =
                            form.NidNumber,

                        Address =
                            form.Address,


                        // -------------------------
                        // EDUCATION STATUS
                        // -------------------------

                        IsBelowSsc =
                            form.IsBelowSsc,


                        // -------------------------
                        // ADMISSION INFORMATION
                        // -------------------------

                        AdmissionDate =
                            form.AdmissionDate,

                        IsActive =
                            form.IsActive,


                        // -------------------------
                        // EDUCATION QUALIFICATIONS
                        // -------------------------

                        EducationQualifications =
                            form.EducationQualifications
                                .Select(item =>
                                    new StudentEducationRequest
                                    {
                                        EducationLevel =
                                            item.EducationLevel,

                                        ExaminationName =
                                            item.ExaminationName,

                                        PassingYear =
                                            item.PassingYear,

                                        Result =
                                            item.Result,

                                        BoardOrUniversity =
                                            item.BoardOrUniversity,

                                        InstitutionName =
                                            item.InstitutionName,

                                        Remarks =
                                            item.Remarks,

                                        DisplayOrder =
                                            item.DisplayOrder
                                    })
                                .ToList(),


                        // -------------------------
                        // PROFESSIONAL EXPERIENCES
                        // -------------------------

                        ProfessionalExperiences =
                            form.ProfessionalExperiences
                                .Select(item =>
                                    new StudentProfessionalExperienceRequest
                                    {
                                        ExperienceTitle =
                                            item.ExperienceTitle,

                                        OrganizationName =
                                            item.OrganizationName,

                                        YearsOfExperience =
                                            item.YearsOfExperience,

                                        Remarks =
                                            item.Remarks
                                    })
                                .ToList(),


                        // -------------------------
                        // LANGUAGE PROFICIENCIES
                        // -------------------------

                        LanguageProficiencies =
                            form.LanguageProficiencies
                                .Select(item =>
                                    new StudentLanguageProficiencyRequest
                                    {
                                        LanguageName =
                                            item.LanguageName,

                                        ProficiencyLevel =
                                            item.ProficiencyLevel,

                                        InstitutionName =
                                            item.InstitutionName,

                                        Remarks =
                                            item.Remarks
                                    })
                                .ToList()
                    };


                // =================================
                // VALIDATE REQUEST
                // =================================

                var validationResult =
                    await _createStudentRequestValidator
                        .ValidateAsync(
                            request,
                            cancellationToken);

                if (!validationResult.IsValid)
                {
                    var errors =
                        validationResult.Errors
                            .GroupBy(error =>
                                error.PropertyName)
                            .ToDictionary(
                                group =>
                                    group.Key,
                                group =>
                                    group
                                        .Select(error =>
                                            error.ErrorMessage)
                                        .Distinct()
                                        .ToArray());

                    return ValidationProblem(
                        new ValidationProblemDetails(
                            errors)
                        {
                            Title =
                                "Student validation failed.",

                            Status =
                                StatusCodes.Status400BadRequest
                        });
                }


                // =================================
                // STUDENT PHOTO
                // =================================

                if (form.StudentPhoto is not null)
                {
                    await using var stream =
                        form.StudentPhoto
                            .OpenReadStream();

                    photoPath =
                        await _fileStorageService
                            .SaveFileAsync(
                                stream,
                                form.StudentPhoto.FileName,
                                "uploads/students/photos",
                                cancellationToken);

                    request.PhotoPath =
                        photoPath;
                }


                // =================================
                // CREATE STUDENT
                // =================================

                var student =
                    await _studentService
                        .CreateAsync(
                            request,
                            cancellationToken);


                // =================================
                // RESPONSE
                // =================================

                return CreatedAtAction(
                    nameof(GetById),
                    new
                    {
                        id = student.Id
                    },
                    student);
            }
            catch
            {
                // =================================
                // DELETE ORPHAN STUDENT PHOTO
                // =================================

                if (!string.IsNullOrWhiteSpace(
                    photoPath))
                {
                    await _fileStorageService
                        .DeleteFileAsync(
                            photoPath,
                            CancellationToken.None);
                }

                throw;
            }
        }


        // =========================================
        // STUDENT SUMMARY
        // GET: api/Students/summary
        // =========================================

        [HttpGet("summary")]
        public async Task<ActionResult<StudentSummaryDto>>
            GetSummary(
                CancellationToken cancellationToken)
        {
            var summary =
                await _studentService
                    .GetSummaryAsync(
                        cancellationToken);

            return Ok(
                summary);
        }


        // =========================================
        // DUE STUDENTS
        // GET: api/Students/due
        // =========================================

        [HttpGet("due")]
        public async Task<
            ActionResult<IReadOnlyList<DueStudentDto>>>
            GetDueStudents(
                CancellationToken cancellationToken)
        {
            var students =
                await _studentService
                    .GetDueStudentsAsync(
                        cancellationToken);

            return Ok(
                students);
        }


        // =========================================
        // PAID STUDENTS
        // GET: api/Students/paid
        // =========================================

        [HttpGet("paid")]
        public async Task<
            ActionResult<IReadOnlyList<PaidStudentDto>>>
            GetPaidStudents(
                CancellationToken cancellationToken)
        {
            var students =
                await _studentService
                    .GetPaidStudentsAsync(
                        cancellationToken);

            return Ok(
                students);
        }


        // =========================================
        // TODAY'S DUE COLLECTION
        // GET: api/Students/due-today
        // =========================================

        [HttpGet("due-today")]
        public async Task<
            ActionResult<IReadOnlyList<TodayDueStudentDto>>>
            GetTodayDueStudents(
                CancellationToken cancellationToken)
        {
            var students =
                await _studentService
                    .GetTodayDueStudentsAsync(
                        cancellationToken);

            return Ok(
                students);
        }
    }
}