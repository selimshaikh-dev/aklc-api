namespace AKLC.Application.Interfaces
{
    public interface IVideoThumbnailService
    {
        // =========================================
        // GENERATE VIDEO THUMBNAIL
        // =========================================
        //
        // videoPath:
        // Relative web path of the uploaded MP4.
        //
        // Example:
        // /uploads/videos/abc123.mp4
        //
        // Returns:
        // Relative web path of the generated image.
        //
        // Example:
        // /uploads/videos/thumbnails/xyz789.jpg
        // =========================================

        Task<string?> GenerateThumbnailAsync(
            string videoPath,
            CancellationToken cancellationToken = default);
    }
}