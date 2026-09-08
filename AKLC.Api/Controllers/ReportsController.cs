using AKLC.Application.DTOs.Reports;
using AKLC.Application.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AKLC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ReportsController
        : ControllerBase
    {
        private readonly IAccountReportService
            _accountReportService;


        public ReportsController(
            IAccountReportService accountReportService)
        {
            _accountReportService =
                accountReportService;
        }


        // =========================================
        // DAILY ACCOUNT REPORT
        // GET: api/Reports/daily
        // =========================================

        [HttpGet("daily")]
        public async Task<ActionResult<AccountReportDto>>
            GetDaily(
                CancellationToken cancellationToken)
        {
            var report =
                await _accountReportService
                    .GetDailyAsync(
                        cancellationToken);


            return Ok(
                report);
        }


        // =========================================
        // DATEWISE ACCOUNT REPORT
        // GET:
        // api/Reports/datewise?from=2026-08-01&to=2026-08-31
        // =========================================

        [HttpGet("datewise")]
        public async Task<ActionResult<AccountReportDto>>
            GetDatewise(
                [FromQuery] DateTime from,
                [FromQuery] DateTime to,
                CancellationToken cancellationToken)
        {
            var report =
                await _accountReportService
                    .GetDatewiseAsync(
                        from,
                        to,
                        cancellationToken);


            return Ok(
                report);
        }
    }
}