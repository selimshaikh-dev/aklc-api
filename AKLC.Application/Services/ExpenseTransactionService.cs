using AKLC.Application.DTOs.Accounts;
using AKLC.Application.Interfaces;
using AKLC.Domain.Entities;

namespace AKLC.Application.Services
{
    public class ExpenseTransactionService
        : IExpenseTransactionService
    {
        private readonly IExpenseTransactionRepository
            _expenseRepository;

        private readonly IAccountHeadRepository
            _accountHeadRepository;


        public ExpenseTransactionService(
            IExpenseTransactionRepository expenseRepository,
            IAccountHeadRepository accountHeadRepository)
        {
            _expenseRepository =
                expenseRepository;

            _accountHeadRepository =
                accountHeadRepository;
        }


        // =========================================
        // GET ALL
        // =========================================

        public async Task<IReadOnlyList<ExpenseTransactionDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var transactions =
                await _expenseRepository
                    .GetAllAsync(
                        cancellationToken);


            return transactions
                .Select(MapToDto)
                .ToList();
        }


        // =========================================
        // GET BY ID
        // =========================================

        public async Task<ExpenseTransactionDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                return null;
            }


            var transaction =
                await _expenseRepository
                    .GetByIdAsync(
                        id,
                        cancellationToken);


            return transaction is null
                ? null
                : MapToDto(transaction);
        }


        // =========================================
        // CREATE
        // =========================================

        public async Task<ExpenseTransactionDto> CreateAsync(
            CreateExpenseTransactionRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request.AccountHeadId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Expense account head is required.");
            }


            if (request.Amount <= 0)
            {
                throw new ArgumentException(
                    "Expense amount must be greater than zero.");
            }


            if (string.IsNullOrWhiteSpace(
                request.PaymentMethod))
            {
                throw new ArgumentException(
                    "Payment method is required.");
            }


            var accountHead =
                await _accountHeadRepository
                    .GetByIdAsync(
                        request.AccountHeadId,
                        cancellationToken);


            if (accountHead is null)
            {
                throw new InvalidOperationException(
                    "Account head was not found.");
            }


            if (
                accountHead.IsDeleted ||
                !accountHead.IsActive
            )
            {
                throw new InvalidOperationException(
                    "Selected account head is inactive.");
            }


            if (
                !accountHead.Type.Equals(
                    "Expense",
                    StringComparison.OrdinalIgnoreCase)
            )
            {
                throw new InvalidOperationException(
                    "Selected account head is not an Expense account.");
            }


            var voucherNumber =
                await GenerateVoucherNumberAsync(
                    cancellationToken);


            var transaction =
                new ExpenseTransaction
                {
                    Id =
                        Guid.NewGuid(),

                    VoucherNumber =
                        voucherNumber,

                    AccountHeadId =
                        accountHead.Id,

                    Amount =
                        request.Amount,

                    TransactionDate =
                        request.TransactionDate == default
                            ? DateTime.UtcNow
                            : request.TransactionDate,

                    PaymentMethod =
                        request.PaymentMethod.Trim(),

                    ReferenceNumber =
                        Normalize(
                            request.ReferenceNumber),

                    AttachmentPath =
                        Normalize(
                            request.AttachmentPath),

                    Remarks =
                        Normalize(
                            request.Remarks),

                    CreatedBy =
                        request.CreatedBy,

                    CreatedAt =
                        DateTime.UtcNow,

                    IsDeleted =
                        false,

                    AccountHead =
                        accountHead
                };


            await _expenseRepository
                .AddAsync(
                    transaction,
                    cancellationToken);


            await _expenseRepository
                .SaveChangesAsync(
                    cancellationToken);


            return MapToDto(
                transaction);
        }


        // =========================================
        // GENERATE VOUCHER NUMBER
        // =========================================

        private async Task<string> GenerateVoucherNumberAsync(
            CancellationToken cancellationToken)
        {
            string voucherNumber;

            do
            {
                voucherNumber =
                    $"EXP-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
            }
            while (
                await _expenseRepository
                    .ExistsByVoucherNumberAsync(
                        voucherNumber,
                        cancellationToken)
            );


            return voucherNumber;
        }


        // =========================================
        // MAP TO DTO
        // =========================================

        private static ExpenseTransactionDto MapToDto(
            ExpenseTransaction transaction)
        {
            return new ExpenseTransactionDto
            {
                Id =
                    transaction.Id,

                VoucherNumber =
                    transaction.VoucherNumber,

                AccountHeadId =
                    transaction.AccountHeadId,

                AccountHeadName =
                    transaction.AccountHead?.Name
                    ?? string.Empty,

                Amount =
                    transaction.Amount,

                TransactionDate =
                    transaction.TransactionDate,

                PaymentMethod =
                    transaction.PaymentMethod,

                ReferenceNumber =
                    transaction.ReferenceNumber,

                AttachmentPath =
                    transaction.AttachmentPath,

                Remarks =
                    transaction.Remarks,

                CreatedBy =
                    transaction.CreatedBy,

                CreatedAt =
                    transaction.CreatedAt
            };
        }


        // =========================================
        // NORMALIZE
        // =========================================

        private static string? Normalize(
            string? value)
        {
            return string.IsNullOrWhiteSpace(
                value)
                ? null
                : value.Trim();
        }
    }
}