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
        // EXISTS
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
    }
}