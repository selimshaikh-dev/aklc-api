using AKLC.Domain.Entities;

namespace AKLC.Application.DTOs.Videos
{
    public class VideoDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public VideoSourceType SourceType { get; set; }

        public string? VideoPath { get; set; }

        public string? VideoUrl { get; set; }

        public string? ThumbnailPath { get; set; }

        public bool HasCustomThumbnail { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}