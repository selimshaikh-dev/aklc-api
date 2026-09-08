using AKLC.Application.DTOs.Accounts;
using AKLC.Application.Interfaces;
using AKLC.Domain.Entities;

namespace AKLC.Application.Services
{
    public class AccountHeadService
        : IAccountHeadService
    {
        private readonly IAccountHeadRepository
            _accountHeadRepository;


        public AccountHeadService(
            IAccountHeadRepository accountHeadRepository)
        {
            _accountHeadRepository =
                accountHeadRepository;
        }


        // =========================================
        // GET ALL
        // =========================================

        public async Task<IReadOnlyList<AccountHeadDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var accountHeads =
                await _accountHeadRepository
                    .GetAllAsync(
                        cancellationToken);


            return accountHeads
                .Select(MapToDto)
                .ToList();
        }


        // =========================================
        // GET BY ID
        // =========================================

        public async Task<AccountHeadDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                return null;
            }


            var accountHead =
                await _accountHeadRepository
                    .GetByIdAsync(
                        id,
                        cancellationToken);


            return accountHead is null
                ? null
                : MapToDto(accountHead);
        }


        // =========================================
        // CREATE
        // =========================================

        public async Task<AccountHeadDto> CreateAsync(
            CreateAccountHeadRequest request,
            CancellationToken cancellationToken = default)
        {
            var name =
                request.Name?.Trim();


            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "Account head name is required.");
            }


            var type =
                request.Type?.Trim();


            if (string.IsNullOrWhiteSpace(type))
            {
                throw new ArgumentException(
                    "Account head type is required.");
            }


            if (
                !type.Equals(
                    "Income",
                    StringComparison.OrdinalIgnoreCase) &&
                !type.Equals(
                    "Expense",
                    StringComparison.OrdinalIgnoreCase)
            )
            {
                throw new ArgumentException(
                    "Account head type must be Income or Expense.");
            }


            type =
                type.Equals(
                    "Income",
                    StringComparison.OrdinalIgnoreCase)
                    ? "Income"
                    : "Expense";


            var exists =
                await _accountHeadRepository
                    .ExistsAsync(
                        name,
                        type,
                        cancellationToken);


            if (exists)
            {
                throw new InvalidOperationException(
                    "An account head with the same name and type already exists.");
            }


            var accountHead =
                new AccountHead
                {
                    Id =
                        Guid.NewGuid(),

                    Name =
                        name,

                    Type =
                        type,

                    IsActive =
                        true,

                    IsDeleted =
                        false,

                    CreatedAt =
                        DateTime.UtcNow
                };


            await _accountHeadRepository
                .AddAsync(
                    accountHead,
                    cancellationToken);


            await _accountHeadRepository
                .SaveChangesAsync(
                    cancellationToken);


            return MapToDto(
                accountHead);
        }


        // =========================================
        // MAP TO DTO
        // =========================================

        private static AccountHeadDto MapToDto(
            AccountHead accountHead)
        {
            return new AccountHeadDto
            {
                Id =
                    accountHead.Id,

                Name =
                    accountHead.Name,

                Type =
                    accountHead.Type,

                IsActive =
                    accountHead.IsActive,

                CreatedAt =
                    accountHead.CreatedAt
            };
        }
    }
}