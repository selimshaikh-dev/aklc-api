using AKLC.Domain.Entities;

namespace AKLC.Application.Interfaces
{
    public interface IVideoRepository
    {
        // =========================================
        // ADMIN
        // =========================================

        Task<IReadOnlyList<Video>> GetAllAsync(
            CancellationToken cancellationToken = default);


        // =========================================
        // PUBLIC WEBSITE
        // =========================================
        // Returns only:
        // - Active videos
        // - Non-deleted videos
        //
        // Latest videos will be ordered by
        // CreatedAt descending in the repository.
        // =========================================

        Task<IReadOnlyList<Video>> GetAllActiveAsync(
            CancellationToken cancellationToken = default);


        // =========================================
        // SINGLE VIDEO
        // =========================================

        Task<Video?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);


        // =========================================
        // EXISTS
        // =========================================

        Task<bool> ExistsAsync(
            Guid id,
            CancellationToken cancellationToken = default);


        // =========================================
        // CREATE
        // =========================================

        Task AddAsync(
            Video video,
            CancellationToken cancellationToken = default);


        // =========================================
        // SAVE
        // =========================================

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}