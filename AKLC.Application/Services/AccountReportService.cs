using AKLC.Application.DTOs.Reports;
using AKLC.Application.Interfaces;

namespace AKLC.Application.Services
{
    public class AccountReportService
        : IAccountReportService
    {
        private readonly IAccountReportRepository
            _accountReportRepository;


        // =========================================
        // CONSTRUCTOR
        // =========================================

        public AccountReportService(
            IAccountReportRepository accountReportRepository)
        {
            _accountReportRepository =
                accountReportRepository;
        }


        // =========================================
        // DAILY REPORT
        // =========================================

        public async Task<AccountReportDto>
            GetDailyAsync(
                CancellationToken cancellationToken = default)
        {
            var bangladeshTimeZone =
                GetBangladeshTimeZone();


            var bangladeshNow =
                TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    bangladeshTimeZone);


            var fromDate =
                bangladeshNow.Date;


            var toDate =
                fromDate.AddDays(1);


            return await _accountReportRepository
                .GetAsync(
                    fromDate,
                    toDate,
                    cancellationToken);
        }


        // =========================================
        // DATEWISE REPORT
        // =========================================

        public async Task<AccountReportDto>
            GetDatewiseAsync(
                DateTime fromDate,
                DateTime toDate,
                CancellationToken cancellationToken = default)
        {
            if (fromDate == default)
            {
                throw new ArgumentException(
                    "From date is required.");
            }


            if (toDate == default)
            {
                throw new ArgumentException(
                    "To date is required.");
            }


            var normalizedFrom =
                fromDate.Date;


            var normalizedToExclusive =
                toDate.Date.AddDays(1);


            if (
                normalizedToExclusive <=
                normalizedFrom
            )
            {
                throw new ArgumentException(
                    "To date must be the same as or later than from date.");
            }


            return await _accountReportRepository
                .GetAsync(
                    normalizedFrom,
                    normalizedToExclusive,
                    cancellationToken);
        }


        // =========================================
        // BANGLADESH TIME ZONE
        // =========================================

        private static TimeZoneInfo
            GetBangladeshTimeZone()
        {
            try
            {
                return TimeZoneInfo
                    .FindSystemTimeZoneById(
                        "Asia/Dhaka");
            }
            catch (
                TimeZoneNotFoundException)
            {
                return TimeZoneInfo
                    .FindSystemTimeZoneById(
                        "Bangladesh Standard Time");
            }
            catch (
                InvalidTimeZoneException)
            {
                return TimeZoneInfo
                    .FindSystemTimeZoneById(
                        "Bangladesh Standard Time");
            }
        }
    }
}