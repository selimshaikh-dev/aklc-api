namespace AKLC.Application.DTOs.Accounts
{
    public class ExpenseTransactionDto
    {
        public Guid Id { get; set; }

        public string VoucherNumber { get; set; } =
            string.Empty;

        public Guid AccountHeadId { get; set; }

        public string AccountHeadName { get; set; } =
            string.Empty;

        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; }

        public string PaymentMethod { get; set; } =
            string.Empty;

        public string? ReferenceNumber { get; set; }

        public string? AttachmentPath { get; set; }

        public string? Remarks { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}