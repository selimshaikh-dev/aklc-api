using AKLC.Application.DTOs.Reports;

namespace AKLC.Application.Interfaces
{
    public interface IAccountReportService
    {
        Task<AccountReportDto> GetDailyAsync(
            CancellationToken cancellationToken = default);

        Task<AccountReportDto> GetDatewiseAsync(
            DateTime fromDate,
            DateTime toDate,
            CancellationToken cancellationToken = default);
    }
}