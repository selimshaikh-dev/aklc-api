using System.Security.Claims;

using AKLC.Api.Models.StudentPayments;
using AKLC.Application.DTOs.StudentPayments;
using AKLC.Application.Interfaces;
using AKLC.Domain.Constants;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AKLC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StudentPaymentsController
        : ControllerBase
    {
        private const long MaxAttachmentFileSize =
            5 * 1024 * 1024;

        private const int MaxAttachmentCount =
            5;

        private static readonly string[]
            AllowedExtensions =
            [
                ".jpg",
                ".jpeg",
                ".png",
                ".webp",
                ".pdf"
            ];

        private static readonly string[]
            AllowedContentTypes =
            [
                "image/jpeg",
                "image/png",
                "image/webp",
                "application/pdf"
            ];

        private readonly IStudentPaymentService
            _studentPaymentService;

        private readonly IFileStorageService
            _fileStorageService;


        // =========================================
        // CONSTRUCTOR
        // =========================================

        public StudentPaymentsController(
            IStudentPaymentService studentPaymentService,
            IFileStorageService fileStorageService)
        {
            _studentPaymentService =
                studentPaymentService;

            _fileStorageService =
                fileStorageService;
        }


        // =========================================
        // RECEIVE PAYMENT
        // POST: api/StudentPayments
        // multipart/form-data
        // =========================================

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<StudentPaymentDto>>
            Create(
                [FromForm] CreateStudentPaymentForm form,
                CancellationToken cancellationToken)
        {
            if (
                form.Attachments.Count >
                MaxAttachmentCount
            )
            {
                return BadRequest(
                    new
                    {
                        message =
                            $"Maximum {MaxAttachmentCount} attachments are allowed."
                    });
            }

            foreach (var file in form.Attachments)
            {
                var validationError =
                    ValidateAttachment(
                        file);

                if (validationError is not null)
                {
                    return BadRequest(
                        new
                        {
                            message =
                                validationError
                        });
                }
            }

            var savedFilePaths =
                new List<string>();

            try
            {
                var request =
                    new CreateStudentPaymentRequest
                    {
                        StudentId =
                            form.StudentId,

                        Amount =
                            form.Amount,

                        PaymentDate =
                            form.PaymentDate,

                        PaymentMethod =
                            form.PaymentMethod,

                        ReferenceNumber =
                            form.ReferenceNumber,

                        Remarks =
                            form.Remarks
                    };

                foreach (var file in form.Attachments)
                {
                    await using var stream =
                        file.OpenReadStream();

                    var filePath =
                        await _fileStorageService
                            .SaveFileAsync(
                                stream,
                                file.FileName,
                                "uploads/payments/attachments",
                                cancellationToken);

                    if (string.IsNullOrWhiteSpace(
                        filePath))
                    {
                        throw new InvalidOperationException(
                            $"Failed to save attachment: {file.FileName}");
                    }

                    savedFilePaths.Add(
                        filePath);

                    var storedFileName =
                        Path.GetFileName(
                            filePath);

                    request.Attachments =
                        request.Attachments
                            .Append(
                                new CreatePaymentAttachmentRequest
                                {
                                    OriginalFileName =
                                        Path.GetFileName(
                                            file.FileName),

                                    StoredFileName =
                                        storedFileName,

                                    FilePath =
                                        filePath,

                                    ContentType =
                                        file.ContentType,

                                    FileSize =
                                        file.Length
                                })
                            .ToList();
                }

                var payment =
                    await _studentPaymentService
                        .CreateAsync(
                            request,
                            cancellationToken);

                return Ok(
                    payment);
            }
            catch
            {
                foreach (var filePath in savedFilePaths)
                {
                    try
                    {
                        await _fileStorageService
                            .DeleteFileAsync(
                                filePath,
                                CancellationToken.None);
                    }
                    catch
                    {
                        // Do not hide the original
                        // payment/database exception.
                    }
                }

                throw;
            }
        }


        // =========================================
        // GET PAYMENT BY ID
        // GET: api/StudentPayments/{id}
        // =========================================

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<StudentPaymentDto>>
            GetById(
                Guid id,
                CancellationToken cancellationToken)
        {
            var payment =
                await _studentPaymentService
                    .GetByIdAsync(
                        id,
                        cancellationToken);

            if (payment is null)
            {
                return NotFound(
                    new
                    {
                        message =
                            "Student payment was not found."
                    });
            }

            return Ok(
                payment);
        }


        // =========================================
        // GET PAYMENT HISTORY BY STUDENT
        // GET: api/StudentPayments/student/{studentId}
        // =========================================

        [HttpGet("student/{studentId:guid}")]
        public async Task<
            ActionResult<IReadOnlyList<StudentPaymentDto>>>
            GetByStudentId(
                Guid studentId,
                CancellationToken cancellationToken)
        {
            var payments =
                await _studentPaymentService
                    .GetByStudentIdAsync(
                        studentId,
                        cancellationToken);

            return Ok(
                payments);
        }


        // =========================================
        // REVERSE PAYMENT
        // POST: api/StudentPayments/{id}/reverse
        // ADMIN ONLY
        // =========================================

        [HttpPost("{id:guid}/reverse")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<ActionResult<StudentPaymentDto>>
            Reverse(
                Guid id,
                [FromBody] ReverseStudentPaymentRequest request,
                CancellationToken cancellationToken)
        {
            var userIdValue =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (
                string.IsNullOrWhiteSpace(
                    userIdValue) ||
                !Guid.TryParse(
                    userIdValue,
                    out var userId)
            )
            {
                return Unauthorized(
                    new
                    {
                        message =
                            "Authenticated user id was not found."
                    });
            }

            var payment =
                await _studentPaymentService
                    .ReverseAsync(
                        id,
                        request,
                        userId,
                        cancellationToken);

            return Ok(
                payment);
        }


        // =========================================
        // VALIDATE ATTACHMENT
        // =========================================

        private static string?
            ValidateAttachment(
                IFormFile file)
        {
            if (file.Length <= 0)
            {
                return
                    "Attachment file is empty.";
            }

            if (
                file.Length >
                MaxAttachmentFileSize
            )
            {
                return
                    "Each attachment cannot exceed 5 MB.";
            }

            var extension =
                Path.GetExtension(
                    file.FileName)
                    .ToLowerInvariant();

            if (
                !AllowedExtensions.Contains(
                    extension)
            )
            {
                return
                    "Attachment must be JPG, JPEG, PNG, WEBP or PDF.";
            }

            var contentType =
                file.ContentType?
                    .ToLowerInvariant()
                ?? string.Empty;

            if (
                !AllowedContentTypes.Contains(
                    contentType)
            )
            {
                return
                    "Invalid attachment file type.";
            }

            return null;
        }
    }
}