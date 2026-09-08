using AKLC.Application.DTOs.PaymentAllocations;
using AKLC.Application.Interfaces;
using AKLC.Domain.Constants;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AKLC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Admin)]
    public class PaymentAllocationsController : ControllerBase
    {
        private readonly IPaymentAllocationService
            _paymentAllocationService;


        // =========================================
        // CONSTRUCTOR
        // =========================================

        public PaymentAllocationsController(
            IPaymentAllocationService paymentAllocationService)
        {
            _paymentAllocationService =
                paymentAllocationService;
        }


        // =========================================
        // CREATE PAYMENT ALLOCATION
        // POST: api/PaymentAllocations
        // =========================================

        [HttpPost]
        public async Task<ActionResult<PaymentAllocationDto>>
            Create(
                [FromBody] CreatePaymentAllocationRequest request,
                CancellationToken cancellationToken)
        {
            var allocation =
                await _paymentAllocationService
                    .CreateAsync(
                        request,
                        cancellationToken);

            return Ok(allocation);
        }


        // =========================================
        // GET ALLOCATIONS BY PAYMENT
        // GET:
        // api/PaymentAllocations/payment/{paymentId}
        // =========================================

        [HttpGet("payment/{studentPaymentId:guid}")]
        public async Task<
            ActionResult<IReadOnlyList<PaymentAllocationDto>>>
            GetByPaymentId(
                Guid studentPaymentId,
                CancellationToken cancellationToken)
        {
            var allocations =
                await _paymentAllocationService
                    .GetByPaymentIdAsync(
                        studentPaymentId,
                        cancellationToken);

            return Ok(allocations);
        }


        // =========================================
        // GET ALLOCATIONS BY SCHEDULE
        // GET:
        // api/PaymentAllocations/schedule/{scheduleId}
        // =========================================

        [HttpGet("schedule/{paymentScheduleId:guid}")]
        public async Task<
            ActionResult<IReadOnlyList<PaymentAllocationDto>>>
            GetByScheduleId(
                Guid paymentScheduleId,
                CancellationToken cancellationToken)
        {
            var allocations =
                await _paymentAllocationService
                    .GetByScheduleIdAsync(
                        paymentScheduleId,
                        cancellationToken);

            return Ok(allocations);
        }
    }
}