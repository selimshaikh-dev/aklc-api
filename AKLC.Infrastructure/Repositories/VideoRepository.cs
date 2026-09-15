using AKLC.Application.Interfaces;
using AKLC.Domain.Entities;
using AKLC.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace AKLC.Infrastructure.Repositories
{
    public class VideoRepository
        : IVideoRepository
    {
        private readonly ApplicationDbContext
            _context;

        public VideoRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }


        // =========================================
        // GET ALL
        // =========================================
        // Admin side:
        // - Active + inactive
        // - Excludes soft-deleted records
        // - Latest videos first
        // =========================================

        public async Task<IReadOnlyList<Video>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            return await _context.Videos
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted)
                .OrderByDescending(x =>
                    x.CreatedAt)
                .ThenBy(x =>
                    x.DisplayOrder)
                .ToListAsync(
                    cancellationToken);
        }


        // =========================================
        // GET ALL ACTIVE
        // =========================================
        // Public website:
        // - Active only
        // - Excludes soft-deleted records
        // - Latest videos first
        //
        // Angular carousel will initially show
        // the first 3 items from this result.
        // =========================================

        public async Task<IReadOnlyList<Video>>
            GetAllActiveAsync(
                CancellationToken cancellationToken = default)
        {
            return await _context.Videos
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    !x.IsDeleted)
                .OrderByDescending(x =>
                    x.CreatedAt)
                .ThenBy(x =>
                    x.DisplayOrder)
                .ToListAsync(
                    cancellationToken);
        }


        // =========================================
        // GET BY ID
        // =========================================

        public async Task<Video?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Videos
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == id &&
                        !x.IsDeleted,
                    cancellationToken);
        }


        // =========================================
        // EXISTS BY ID
        // =========================================

        public async Task<bool> ExistsAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Videos
                .AsNoTracking()
                .AnyAsync(
                    x =>
                        x.Id == id &&
                        !x.IsDeleted,
                    cancellationToken);
        }


        // =========================================
        // ADD
        // =========================================

        public async Task AddAsync(
            Video video,
            CancellationToken cancellationToken = default)
        {
            await _context.Videos
                .AddAsync(
                    video,
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