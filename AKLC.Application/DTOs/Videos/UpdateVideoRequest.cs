using AKLC.Domain.Entities;

namespace AKLC.Application.DTOs.Videos
{
    public class UpdateVideoRequest
    {
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public VideoSourceType SourceType { get; set; }

        public string? VideoPath { get; set; }

        public string? VideoUrl { get; set; }

        public string? ThumbnailPath { get; set; }

        public bool HasCustomThumbnail { get; set; } = false;

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }
}