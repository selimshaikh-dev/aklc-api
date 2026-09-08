using AKLC.Application.Interfaces;
using AKLC.Domain.Entities;
using AKLC.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AKLC.Infrastructure.Repositories
{
    public class AccountHeadRepository
        : IAccountHeadRepository
    {
        private readonly ApplicationDbContext
            _context;


        // =========================================
        // CONSTRUCTOR
        // =========================================

        public AccountHeadRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }


        // =========================================
        // GET ALL
        // =========================================

        public async Task<IReadOnlyList<AccountHead>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            return await _context.AccountHeads
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted)
                .OrderBy(x =>
                    x.Type)
                .ThenBy(x =>
                    x.Name)
                .ToListAsync(
                    cancellationToken);
        }


        // =========================================
        // GET BY ID
        // READ ONLY
        // =========================================

        public async Task<AccountHead?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.AccountHeads
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == id &&
                        !x.IsDeleted,
                    cancellationToken);
        }


        // =========================================
        // GET BY ID FOR UPDATE
        // TRACKED
        // =========================================

        public async Task<AccountHead?>
            GetByIdForUpdateAsync(
                Guid id,
                CancellationToken cancellationToken = default)
        {
            return await _context.AccountHeads
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == id &&
                        !x.IsDeleted,
                    cancellationToken);
        }


        // =========================================
        // EXISTS
        // =========================================

        public async Task<bool> ExistsAsync(
            string name,
            string type,
            CancellationToken cancellationToken = default)
        {
            var normalizedName =
                name.Trim();

            var normalizedType =
                type.Trim();


            return await _context.AccountHeads
                .AsNoTracking()
                .AnyAsync(
                    x =>
                        !x.IsDeleted &&
                        x.Name == normalizedName &&
                        x.Type == normalizedType,
                    cancellationToken);
        }


        // =========================================
        // ADD
        // =========================================

        public async Task AddAsync(
            AccountHead accountHead,
            CancellationToken cancellationToken = default)
        {
            await _context.AccountHeads
                .AddAsync(
                    accountHead,
                    cancellationToken);
        }


        // =========================================
        // SAVE CHANGES
        // =========================================

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await _context
                .SaveChangesAsync(
                    cancellationToken);
        }
    }
}