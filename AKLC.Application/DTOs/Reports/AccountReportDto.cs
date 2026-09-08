namespace AKLC.Application.DTOs.Reports
{
    public class AccountReportDto
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public decimal StudentPaymentIncome { get; set; }

        public decimal ManualIncome { get; set; }

        public decimal TotalIncome { get; set; }

        public decimal TotalExpense { get; set; }

        public decimal NetBalance { get; set; }

        public IReadOnlyList<AccountReportItemDto>
            Transactions
        { get; set; } =
                Array.Empty<AccountReportItemDto>();
    }
}