using System.Security.Claims;

using AKLC.Api.Models.Accounts;

using AKLC.Application.DTOs.Accounts;
using AKLC.Application.Interfaces;

using AKLC.Domain.Constants;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AKLC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Admin)]
    public class IncomeController
        : ControllerBase
    {
        private const long MaxAttachmentSize =
            5 * 1024 * 1024;


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


        private readonly IIncomeTransactionService
            _incomeService;

        private readonly IFileStorageService
            _fileStorageService;


        // =========================================
        // CONSTRUCTOR
        // =========================================

        public IncomeController(
            IIncomeTransactionService incomeService,
            IFileStorageService fileStorageService)
        {
            _incomeService =
                incomeService;

            _fileStorageService =
                fileStorageService;
        }


        // =========================================
        // GET ALL INCOME TRANSACTIONS
        // GET: api/Income
        // =========================================

        [HttpGet]
        public async Task<
            ActionResult<
                IReadOnlyList<IncomeTransactionDto>>>
            GetAll(
                CancellationToken cancellationToken)
        {
            var transactions =
                await _incomeService
                    .GetAllAsync(
                        cancellationToken);


            return Ok(
                transactions);
        }


        // =========================================
        // GET INCOME BY ID
        // GET: api/Income/{id}
        // =========================================

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<IncomeTransactionDto>>
            GetById(
                Guid id,
                CancellationToken cancellationToken)
        {
            var transaction =
                await _incomeService
                    .GetByIdAsync(
                        id,
                        cancellationToken);


            if (transaction is null)
            {
                return NotFound(
                    new
                    {
                        message =
                            "Income transaction was not found."
                    });
            }


            return Ok(
                transaction);
        }


        // =========================================
        // CREATE INCOME
        // POST: api/Income
        // multipart/form-data
        // =========================================

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<IncomeTransactionDto>>
            Create(
                [FromForm] CreateIncomeTransactionForm form,
                CancellationToken cancellationToken)
        {
            var userIdValue =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);


            if (
                !Guid.TryParse(
                    userIdValue,
                    out var createdByUserId)
            )
            {
                return Unauthorized(
                    new
                    {
                        message =
                            "Authenticated user identifier is invalid."
                    });
            }


            string? attachmentPath =
                null;


            // -------------------------------------
            // ATTACHMENT VALIDATION
            // -------------------------------------

            if (form.Attachment is not null)
            {
                var validationError =
                    ValidateAttachment(
                        form.Attachment);


                if (validationError is not null)
                {
                    return BadRequest(
                        new
                        {
                            message =
                                validationError
                        });
                }


                await using var stream =
                    form.Attachment
                        .OpenReadStream();


                attachmentPath =
                    await _fileStorageService
                        .SaveFileAsync(
                            stream,
                            form.Attachment.FileName,
                            "uploads/accounts/income",
                            cancellationToken);
            }


            try
            {
                var request =
                    new CreateIncomeTransactionRequest
                    {
                        AccountHeadId =
                            form.AccountHeadId,

                        Amount =
                            form.Amount,

                        TransactionDate =
                            form.TransactionDate,

                        PaymentMethod =
                            form.PaymentMethod,

                        ReferenceNumber =
                            form.ReferenceNumber,

                        AttachmentPath =
                            attachmentPath,

                        Remarks =
                            form.Remarks,

                        CreatedBy =
                            createdByUserId
                    };


                var transaction =
                    await _incomeService
                        .CreateAsync(
                            request,
                            cancellationToken);


                return CreatedAtAction(
                    nameof(GetById),
                    new
                    {
                        id =
                            transaction.Id
                    },
                    transaction);
            }
            catch
            {
                if (
                    !string.IsNullOrWhiteSpace(
                        attachmentPath)
                )
                {
                    await _fileStorageService
                        .DeleteFileAsync(
                            attachmentPath,
                            CancellationToken.None);
                }


                throw;
            }
        }


        // =========================================
        // VALIDATE ATTACHMENT
        // =========================================

        private static string? ValidateAttachment(
            IFormFile file)
        {
            if (file.Length <= 0)
            {
                return
                    "Attachment file is empty.";
            }


            if (file.Length > MaxAttachmentSize)
            {
                return
                    "Attachment cannot exceed 5 MB.";
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
                file.ContentType
                    .ToLowerInvariant();


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