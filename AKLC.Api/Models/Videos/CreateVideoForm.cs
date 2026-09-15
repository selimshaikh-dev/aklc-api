using AKLC.Domain.Entities;

namespace AKLC.Api.Models.Videos
{
    public class CreateVideoForm
    {
        public string Title { get; set; }
            = string.Empty;

        public string? Description { get; set; }


        // =========================================
        // VIDEO SOURCE
        // =========================================

        public VideoSourceType SourceType { get; set; }


        // =========================================
        // UPLOADED VIDEO
        // =========================================
        // Used only when:
        // SourceType = UploadedFile
        //
        // Expected:
        // MP4
        // =========================================

        public IFormFile? VideoFile { get; set; }


        // =========================================
        // YOUTUBE URL
        // =========================================
        // Used only when:
        // SourceType = YouTube
        // =========================================

        public string? VideoUrl { get; set; }


        // =========================================
        // CUSTOM THUMBNAIL
        // =========================================
        // Optional for both source types.
        //
        // If uploaded:
        // - this thumbnail will be used
        //
        // If not uploaded:
        // - YouTube -> YouTube thumbnail automatically
        // - MP4 -> auto-generated thumbnail later
        // =========================================

        public IFormFile? ThumbnailFile { get; set; }


        // =========================================
        // DISPLAY
        // =========================================

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }
}