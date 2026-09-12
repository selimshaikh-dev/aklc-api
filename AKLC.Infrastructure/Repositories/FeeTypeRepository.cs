using AKLC.Application.Interfaces;
using AKLC.Domain.Entities;
using AKLC.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace AKLC.Infrastructure.Repositories
{
    public class FeeTypeRepository
        : IFeeTypeRepository
    {
        private readonly ApplicationDbContext
            _context;

        public FeeTypeRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }


        // =========================================
        // GET ALL ACTIVE
        // =========================================

        public async Task<IReadOnlyList<FeeType>>
            GetAllActiveAsync(
                CancellationToken cancellationToken = default)
        {
            return await _context.FeeTypes
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    !x.IsDeleted)
                .OrderBy(x => x.Name)
                .ToListAsync(
                    cancellationToken);
        }


        // =========================================
        // GET BY ID
        // =========================================

        public async Task<FeeType?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.FeeTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }


        // =========================================
        // EXISTS BY ID
        // =========================================

        public async Task<bool> ExistsAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.FeeTypes
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == id,
                    cancellationToken);
        }


        // =========================================
        // EXISTS BY NAME
        // =========================================

        public async Task<bool> ExistsByNameAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            var normalizedName =
                name.Trim();

            return await _context.FeeTypes
                .AsNoTracking()
                .AnyAsync(
                    x =>
                        x.Name == normalizedName &&
                        !x.IsDeleted,
                    cancellationToken);
        }


        // =========================================
        // ADD
        // =========================================

        public async Task AddAsync(
            FeeType feeType,
            CancellationToken cancellationToken = default)
        {
            await _context.FeeTypes
                .AddAsync(
                    feeType,
                    cancellationToken);
        }


        // =========================================
        // SAVE CHANGES
        // =========================================

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(
                cancellationToken);
        }
    }
}