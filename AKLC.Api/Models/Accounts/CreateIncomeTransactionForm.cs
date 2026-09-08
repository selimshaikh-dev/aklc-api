namespace AKLC.Api.Models.Accounts
{
    public class CreateIncomeTransactionForm
    {
        public Guid AccountHeadId { get; set; }

        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; }

        public string PaymentMethod { get; set; } =
            string.Empty;

        public string? ReferenceNumber { get; set; }

        public string? Remarks { get; set; }

        public IFormFile? Attachment { get; set; }
    }
}