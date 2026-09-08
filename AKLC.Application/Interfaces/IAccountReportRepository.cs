using AKLC.Application.DTOs.Reports;

namespace AKLC.Application.Interfaces
{
    public interface IAccountReportRepository
    {
        Task<AccountReportDto> GetAsync(
            DateTime fromDate,
            DateTime toDate,
            CancellationToken cancellationToken = default);
    }
}