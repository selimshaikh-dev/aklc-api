using AKLC.Domain.Common;

namespace AKLC.Domain.Entities
{
    public enum VideoSourceType
    {
        UploadedFile = 1,
        YouTube = 2
    }


    public class Video : BaseEntity
    {
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }


        // =========================================
        // VIDEO SOURCE
        // =========================================

        public VideoSourceType SourceType { get; set; }


        // Used only when SourceType = UploadedFile
        //
        // Example:
        // /uploads/videos/lesson-01.mp4
        public string? VideoPath { get; set; }


        // Used only when SourceType = YouTube
        //
        // Example:
        // https://www.youtube.com/watch?v=xxxx
        public string? VideoUrl { get; set; }


        // =========================================
        // THUMBNAIL
        // =========================================

        // For uploaded MP4:
        // - auto-generated thumbnail path
        // - OR manually uploaded custom thumbnail
        //
        // For YouTube:
        // - normally null when YouTube thumbnail is used automatically
        // - may contain a custom uploaded thumbnail path
        public string? ThumbnailPath { get; set; }


        // True when admin manually uploads a thumbnail.
        //
        // False:
        // - MP4 -> system generated thumbnail
        // - YouTube -> YouTube thumbnail
        public bool HasCustomThumbnail { get; set; } = false;


        // =========================================
        // DISPLAY
        // =========================================

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;
    }
}