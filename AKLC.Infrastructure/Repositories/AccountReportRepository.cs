using AKLC.Application.DTOs.Reports;
using AKLC.Application.Interfaces;
using AKLC.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace AKLC.Infrastructure.Repositories
{
    public class AccountReportRepository
        : IAccountReportRepository
    {
        private readonly ApplicationDbContext
            _context;


        public AccountReportRepository(
            ApplicationDbContext context)
        {
            _context =
                context;
        }


        // =========================================
        // GET REPORT
        // =========================================

        public async Task<AccountReportDto> GetAsync(
            DateTime fromDate,
            DateTime toDate,
            CancellationToken cancellationToken = default)
        {
            // =====================================
            // STUDENT PAYMENTS
            // =====================================

            var studentPayments =
                await _context.StudentPayments
                    .AsNoTracking()
                    .Where(payment =>
                        !payment.IsReversed &&
                        payment.PaymentDate >= fromDate &&
                        payment.PaymentDate < toDate)
                    .Select(payment =>
                        new AccountReportItemDto
                        {
                            TransactionDate =
                                payment.PaymentDate,

                            Source =
                                "Student Payment",

                            Type =
                                "Income",

                            Description =
                                "Student payment",

                            VoucherNumber =
                                payment.ReceiptNumber,

                            PaymentMethod =
                                payment.PaymentMethod,

                            Amount =
                                payment.Amount,

                            ReferenceNumber =
                                payment.ReferenceNumber,

                            Remarks =
                                payment.Remarks
                        })
                    .ToListAsync(
                        cancellationToken);


            // =====================================
            // MANUAL INCOME
            // =====================================

            var manualIncome =
                await _context.IncomeTransactions
                    .AsNoTracking()
                    .Include(x =>
                        x.AccountHead)
                    .Where(transaction =>
                        !transaction.IsDeleted &&
                        transaction.TransactionDate >= fromDate &&
                        transaction.TransactionDate < toDate)
                    .Select(transaction =>
                        new AccountReportItemDto
                        {
                            TransactionDate =
                                transaction.TransactionDate,

                            Source =
                                "Manual Income",

                            Type =
                                "Income",

                            Description =
                                transaction.AccountHead.Name,

                            VoucherNumber =
                                transaction.VoucherNumber,

                            PaymentMethod =
                                transaction.PaymentMethod,

                            Amount =
                                transaction.Amount,

                            ReferenceNumber =
                                transaction.ReferenceNumber,

                            Remarks =
                                transaction.Remarks
                        })
                    .ToListAsync(
                        cancellationToken);


            // =====================================
            // EXPENSES
            // =====================================

            var expenses =
                await _context.ExpenseTransactions
                    .AsNoTracking()
                    .Include(x =>
                        x.AccountHead)
                    .Where(transaction =>
                        !transaction.IsDeleted &&
                        transaction.TransactionDate >= fromDate &&
                        transaction.TransactionDate < toDate)
                    .Select(transaction =>
                        new AccountReportItemDto
                        {
                            TransactionDate =
                                transaction.TransactionDate,

                            Source =
                                "Expense",

                            Type =
                                "Expense",

                            Description =
                                transaction.AccountHead.Name,

                            VoucherNumber =
                                transaction.VoucherNumber,

                            PaymentMethod =
                                transaction.PaymentMethod,

                            Amount =
                                transaction.Amount,

                            ReferenceNumber =
                                transaction.ReferenceNumber,

                            Remarks =
                                transaction.Remarks
                        })
                    .ToListAsync(
                        cancellationToken);


            // =====================================
            // TOTALS
            // =====================================

            var studentPaymentIncome =
                studentPayments.Sum(
                    x => x.Amount);


            var manualIncomeTotal =
                manualIncome.Sum(
                    x => x.Amount);


            var totalIncome =
                studentPaymentIncome +
                manualIncomeTotal;


            var totalExpense =
                expenses.Sum(
                    x => x.Amount);


            var netBalance =
                totalIncome -
                totalExpense;


            // =====================================
            // TRANSACTION LIST
            // =====================================

            var transactions =
                studentPayments
                    .Concat(
                        manualIncome)
                    .Concat(
                        expenses)
                    .OrderByDescending(
                        x => x.TransactionDate)
                    .ToList();


            return new AccountReportDto
            {
                FromDate =
                    fromDate,

                ToDate =
                    toDate.AddTicks(-1),

                StudentPaymentIncome =
                    studentPaymentIncome,

                ManualIncome =
                    manualIncomeTotal,

                TotalIncome =
                    totalIncome,

                TotalExpense =
                    totalExpense,

                NetBalance =
                    netBalance,

                Transactions =
                    transactions
            };
        }
    }
}