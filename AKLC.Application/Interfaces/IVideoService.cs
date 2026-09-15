using AKLC.Application.DTOs.Videos;

namespace AKLC.Application.Interfaces
{
    public interface IVideoService
    {
        // =========================================
        // ADMIN
        // =========================================

        Task<IReadOnlyList<VideoDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default);


        // =========================================
        // PUBLIC WEBSITE
        // =========================================
        // Active + non-deleted videos only.
        // Newest videos first.
        // =========================================

        Task<IReadOnlyList<VideoDto>>
            GetAllActiveAsync(
                CancellationToken cancellationToken = default);


        // =========================================
        // GET BY ID
        // =========================================

        Task<VideoDto?>
            GetByIdAsync(
                Guid id,
                CancellationToken cancellationToken = default);


        // =========================================
        // CREATE
        // =========================================

        Task<VideoDto>
            CreateAsync(
                CreateVideoRequest request,
                CancellationToken cancellationToken = default);


        // =========================================
        // UPDATE
        // =========================================

        Task<VideoDto>
            UpdateAsync(
                Guid id,
                UpdateVideoRequest request,
                CancellationToken cancellationToken = default);


        // =========================================
        // ACTIVE / INACTIVE
        // =========================================

        Task SetActiveStatusAsync(
            Guid id,
            bool isActive,
            CancellationToken cancellationToken = default);


        // =========================================
        // SOFT DELETE
        // =========================================

        Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}