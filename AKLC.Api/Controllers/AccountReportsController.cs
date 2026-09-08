using AKLC.Application.DTOs.Reports;
using AKLC.Application.Interfaces;
using AKLC.Domain.Constants;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AKLC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Admin)]
    public class AccountReportsController
        : ControllerBase
    {
        private readonly IAccountReportService
            _accountReportService;


        // =========================================
        // CONSTRUCTOR
        // =========================================

        public AccountReportsController(
            IAccountReportService accountReportService)
        {
            _accountReportService =
                accountReportService;
        }


        // =========================================
        // DAILY ACCOUNT REPORT
        // GET: api/AccountReports/daily
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
        // api/AccountReports/datewise?fromDate=2026-09-01&toDate=2026-09-05
        // =========================================

        [HttpGet("datewise")]
        public async Task<ActionResult<AccountReportDto>>
            GetDatewise(
                [FromQuery] DateTime fromDate,
                [FromQuery] DateTime toDate,
                CancellationToken cancellationToken)
        {
            var report =
                await _accountReportService
                    .GetDatewiseAsync(
                        fromDate,
                        toDate,
                        cancellationToken);

            return Ok(
                report);
        }
    }
}