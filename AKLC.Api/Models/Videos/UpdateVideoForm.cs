using AKLC.Domain.Entities;

namespace AKLC.Api.Models.Videos
{
    public class UpdateVideoForm
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
        // Optional during update.
        //
        // If SourceType = UploadedFile:
        // - upload a new MP4 to replace existing video
        // - OR leave empty to keep existing MP4
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
        // Optional.
        //
        // If uploaded:
        // - replaces current thumbnail
        //
        // If empty:
        // - existing custom thumbnail may remain
        // - or system thumbnail can be used
        // =========================================

        public IFormFile? ThumbnailFile { get; set; }


        // =========================================
        // THUMBNAIL CONTROL
        // =========================================
        // true:
        // remove current custom thumbnail and
        // return to automatic thumbnail behavior
        //
        // YouTube:
        // -> YouTube thumbnail
        //
        // MP4:
        // -> generated thumbnail
        // =========================================

        public bool RemoveCustomThumbnail { get; set; }
            = false;


        // =========================================
        // DISPLAY
        // =========================================

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }
}