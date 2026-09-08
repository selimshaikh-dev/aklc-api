using AKLC.Application.DTOs.Accounts;
using AKLC.Application.Interfaces;
using AKLC.Domain.Entities;

namespace AKLC.Application.Services
{
    public class IncomeTransactionService
        : IIncomeTransactionService
    {
        private readonly IIncomeTransactionRepository
            _incomeRepository;

        private readonly IAccountHeadRepository
            _accountHeadRepository;


        public IncomeTransactionService(
            IIncomeTransactionRepository incomeRepository,
            IAccountHeadRepository accountHeadRepository)
        {
            _incomeRepository =
                incomeRepository;

            _accountHeadRepository =
                accountHeadRepository;
        }


        // =========================================
        // GET ALL
        // =========================================

        public async Task<IReadOnlyList<IncomeTransactionDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var transactions =
                await _incomeRepository
                    .GetAllAsync(
                        cancellationToken);


            return transactions
                .Select(MapToDto)
                .ToList();
        }


        // =========================================
        // GET BY ID
        // =========================================

        public async Task<IncomeTransactionDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                return null;
            }


            var transaction =
                await _incomeRepository
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

        public async Task<IncomeTransactionDto> CreateAsync(
            CreateIncomeTransactionRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request.AccountHeadId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Income account head is required.");
            }


            if (request.Amount <= 0)
            {
                throw new ArgumentException(
                    "Income amount must be greater than zero.");
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
                    "Income",
                    StringComparison.OrdinalIgnoreCase)
            )
            {
                throw new InvalidOperationException(
                    "Selected account head is not an Income account.");
            }


            var voucherNumber =
                await GenerateVoucherNumberAsync(
                    cancellationToken);


            var transaction =
                new IncomeTransaction
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


            await _incomeRepository
                .AddAsync(
                    transaction,
                    cancellationToken);


            await _incomeRepository
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
                    $"INC-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
            }
            while (
                await _incomeRepository
                    .ExistsByVoucherNumberAsync(
                        voucherNumber,
                        cancellationToken)
            );


            return voucherNumber;
        }


        // =========================================
        // MAP TO DTO
        // =========================================

        private static IncomeTransactionDto MapToDto(
            IncomeTransaction transaction)
        {
            return new IncomeTransactionDto
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