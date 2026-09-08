using AKLC.Application.DTOs.PaymentSchedules;
using AKLC.Application.Interfaces;
using AKLC.Domain.Constants;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AKLC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Admin)]
    public class PaymentSchedulesController
        : ControllerBase
    {
        private readonly IPaymentScheduleService
            _paymentScheduleService;


        // =========================================
        // CONSTRUCTOR
        // =========================================

        public PaymentSchedulesController(
            IPaymentScheduleService paymentScheduleService)
        {
            _paymentScheduleService =
                paymentScheduleService;
        }


        // =========================================
        // GET PAYMENT SCHEDULE BY ID
        // GET: api/PaymentSchedules/{id}
        // =========================================

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PaymentScheduleDto>>
            GetById(
                Guid id,
                CancellationToken cancellationToken)
        {
            var schedule =
                await _paymentScheduleService
                    .GetByIdAsync(
                        id,
                        cancellationToken);

            if (schedule is null)
            {
                return NotFound(
                    new
                    {
                        message =
                            "Payment schedule not found."
                    });
            }

            return Ok(
                schedule);
        }


        // =========================================
        // GET SCHEDULES BY STUDENT
        // GET: api/PaymentSchedules/student/{studentId}
        // =========================================

        [HttpGet("student/{studentId:guid}")]
        public async Task<
            ActionResult<IReadOnlyList<PaymentScheduleDto>>>
            GetByStudentId(
                Guid studentId,
                CancellationToken cancellationToken)
        {
            var schedules =
                await _paymentScheduleService
                    .GetByStudentIdAsync(
                        studentId,
                        cancellationToken);

            return Ok(
                schedules);
        }


        // =========================================
        // GET SCHEDULES BY FEE ASSIGNMENT
        // GET:
        // api/PaymentSchedules/fee-assignment/{id}
        // =========================================

        [HttpGet(
            "fee-assignment/{studentFeeAssignmentId:guid}")]
        public async Task<
            ActionResult<IReadOnlyList<PaymentScheduleDto>>>
            GetByFeeAssignmentId(
                Guid studentFeeAssignmentId,
                CancellationToken cancellationToken)
        {
            var schedules =
                await _paymentScheduleService
                    .GetByFeeAssignmentIdAsync(
                        studentFeeAssignmentId,
                        cancellationToken);

            return Ok(
                schedules);
        }


        // =========================================
        // CREATE PAYMENT SCHEDULE
        // POST: api/PaymentSchedules
        // =========================================

        [HttpPost]
        public async Task<ActionResult<PaymentScheduleDto>>
            Create(
                [FromBody]
                CreatePaymentScheduleRequest request,
                CancellationToken cancellationToken)
        {
            var schedule =
                await _paymentScheduleService
                    .CreateAsync(
                        request,
                        cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new
                {
                    id = schedule.Id
                },
                schedule);
        }
    }
}