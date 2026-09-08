namespace AKLC.Application.DTOs.Reports
{
    public class AccountReportItemDto
    {
        public DateTime TransactionDate { get; set; }

        public string Source { get; set; } =
            string.Empty;

        public string Type { get; set; } =
            string.Empty;

        public string Description { get; set; } =
            string.Empty;

        public string? VoucherNumber { get; set; }

        public string PaymentMethod { get; set; } =
            string.Empty;

        public decimal Amount { get; set; }

        public string? ReferenceNumber { get; set; }

        public string? Remarks { get; set; }
    }
}