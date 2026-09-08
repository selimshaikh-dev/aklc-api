namespace AKLC.Application.DTOs.Accounts
{
    public class CreateIncomeTransactionRequest
    {
        public Guid AccountHeadId { get; set; }

        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; }

        public string PaymentMethod { get; set; } =
            string.Empty;

        public string? ReferenceNumber { get; set; }

        public string? AttachmentPath { get; set; }

        public string? Remarks { get; set; }

        public Guid? CreatedBy { get; set; }
    }
}