using AKLC.Application.DTOs.FeeTypes;
using AKLC.Application.Interfaces;
using AKLC.Domain.Constants;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AKLC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Admin)]
    public class FeeTypesController
        : ControllerBase
    {
        private readonly IFeeTypeService
            _feeTypeService;


        // =========================================
        // CONSTRUCTOR
        // =========================================

        public FeeTypesController(
            IFeeTypeService feeTypeService)
        {
            _feeTypeService =
                feeTypeService;
        }


        // =========================================
        // GET ALL ACTIVE FEE TYPES
        // GET: api/FeeTypes
        // =========================================

        [HttpGet]
        public async Task<
            ActionResult<IReadOnlyList<FeeTypeDto>>>
            GetAllActive(
                CancellationToken cancellationToken)
        {
            var feeTypes =
                await _feeTypeService
                    .GetAllActiveAsync(
                        cancellationToken);

            return Ok(
                feeTypes);
        }


        // =========================================
        // CREATE FEE TYPE
        // POST: api/FeeTypes
        // =========================================

        [HttpPost]
        public async Task<ActionResult<FeeTypeDto>>
            Create(
                [FromBody]
                CreateFeeTypeRequest request,
                CancellationToken cancellationToken)
        {
            var feeType =
                await _feeTypeService
                    .CreateAsync(
                        request,
                        cancellationToken);

            return Created(
                $"/api/FeeTypes/{feeType.Id}",
                feeType);
        }
    }
}