using AKLC.Application.Interfaces;
using AKLC.Domain.Entities;
using AKLC.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AKLC.Infrastructure.Repositories
{
    public class IncomeTransactionRepository
        : IIncomeTransactionRepository
    {
        private readonly ApplicationDbContext
            _context;


        // =========================================
        // CONSTRUCTOR
        // =========================================

        public IncomeTransactionRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }


        // =========================================
        // GET ALL
        // =========================================

        public async Task<IReadOnlyList<IncomeTransaction>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            return await _context.IncomeTransactions
                .AsNoTracking()
                .Include(x =>
                    x.AccountHead)
                .Where(x =>
                    !x.IsDeleted)
                .OrderByDescending(x =>
                    x.TransactionDate)
                .ThenByDescending(x =>
                    x.CreatedAt)
                .ToListAsync(
                    cancellationToken);
        }


        // =========================================
        // GET BY ID
        // =========================================

        public async Task<IncomeTransaction?>
            GetByIdAsync(
                Guid id,
                CancellationToken cancellationToken = default)
        {
            return await _context.IncomeTransactions
                .AsNoTracking()
                .Include(x =>
                    x.AccountHead)
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == id &&
                        !x.IsDeleted,
                    cancellationToken);
        }


        // =========================================
        // ADD
        // =========================================

        public async Task AddAsync(
            IncomeTransaction transaction,
            CancellationToken cancellationToken = default)
        {
            await _context.IncomeTransactions
                .AddAsync(
                    transaction,
                    cancellationToken);
        }


        // =========================================
        // CHECK VOUCHER
        // =========================================

        public async Task<bool>
            ExistsByVoucherNumberAsync(
                string voucherNumber,
                CancellationToken cancellationToken = default)
        {
            return await _context.IncomeTransactions
                .AsNoTracking()
                .AnyAsync(
                    x =>
                        x.VoucherNumber ==
                        voucherNumber,
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