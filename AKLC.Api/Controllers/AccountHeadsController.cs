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
    public class AccountHeadsController
        : ControllerBase
    {
        private readonly IAccountHeadService
            _accountHeadService;


        // =========================================
        // CONSTRUCTOR
        // =========================================

        public AccountHeadsController(
            IAccountHeadService accountHeadService)
        {
            _accountHeadService =
                accountHeadService;
        }


        // =========================================
        // GET ALL ACCOUNT HEADS
        // GET: api/AccountHeads
        // =========================================

        [HttpGet]
        public async Task<
            ActionResult<IReadOnlyList<AccountHeadDto>>>
            GetAll(
                CancellationToken cancellationToken)
        {
            var accountHeads =
                await _accountHeadService
                    .GetAllAsync(
                        cancellationToken);


            return Ok(
                accountHeads);
        }


        // =========================================
        // GET ACCOUNT HEAD BY ID
        // GET: api/AccountHeads/{id}
        // =========================================

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AccountHeadDto>>
            GetById(
                Guid id,
                CancellationToken cancellationToken)
        {
            var accountHead =
                await _accountHeadService
                    .GetByIdAsync(
                        id,
                        cancellationToken);


            if (accountHead is null)
            {
                return NotFound(
                    new
                    {
                        message =
                            "Account head was not found."
                    });
            }


            return Ok(
                accountHead);
        }


        // =========================================
        // CREATE ACCOUNT HEAD
        // POST: api/AccountHeads
        // =========================================

        [HttpPost]
        public async Task<ActionResult<AccountHeadDto>>
            Create(
                [FromBody] CreateAccountHeadRequest request,
                CancellationToken cancellationToken)
        {
            var accountHead =
                await _accountHeadService
                    .CreateAsync(
                        request,
                        cancellationToken);


            return CreatedAtAction(
                nameof(GetById),
                new
                {
                    id =
                        accountHead.Id
                },
                accountHead);
        }
    }
}