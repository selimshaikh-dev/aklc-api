namespace AKLC.Domain.Entities
{
    public class AccountHead
    {
        public Guid Id { get; set; }

        public string Name { get; set; } =
            string.Empty;

        // Income / Expense
        public string Type { get; set; } =
            string.Empty;

        public bool IsActive { get; set; } =
            true;

        public bool IsDeleted { get; set; } =
            false;

        public DateTime CreatedAt { get; set; }


        // =========================================
        // NAVIGATION
        // =========================================

        public ICollection<IncomeTransaction>
            IncomeTransactions
        { get; set; } =
                new List<IncomeTransaction>();

        public ICollection<ExpenseTransaction>
            ExpenseTransactions
        { get; set; } =
                new List<ExpenseTransaction>();
    }
}