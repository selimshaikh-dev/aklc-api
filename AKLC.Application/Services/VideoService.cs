using AKLC.Application.DTOs.Videos;
using AKLC.Application.Interfaces;
using AKLC.Domain.Entities;

namespace AKLC.Application.Services
{
    public class VideoService
        : IVideoService
    {
        private readonly IVideoRepository
            _videoRepository;


        // =========================================
        // CONSTRUCTOR
        // =========================================

        public VideoService(
            IVideoRepository videoRepository)
        {
            _videoRepository =
                videoRepository;
        }


        // =========================================
        // GET ALL
        // =========================================
        // Admin:
        // Active + inactive videos.
        // Deleted videos are excluded by repository.
        // =========================================

        public async Task<IReadOnlyList<VideoDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var videos =
                await _videoRepository
                    .GetAllAsync(
                        cancellationToken);

            return videos
                .Select(
                    MapToDto)
                .ToList();
        }


        // =========================================
        // GET ALL ACTIVE
        // =========================================
        // Public website:
        // Active videos only.
        // Repository returns newest first.
        // =========================================

        public async Task<IReadOnlyList<VideoDto>>
            GetAllActiveAsync(
                CancellationToken cancellationToken = default)
        {
            var videos =
                await _videoRepository
                    .GetAllActiveAsync(
                        cancellationToken);

            return videos
                .Select(
                    MapToDto)
                .ToList();
        }


        // =========================================
        // GET BY ID
        // =========================================

        public async Task<VideoDto?>
            GetByIdAsync(
                Guid id,
                CancellationToken cancellationToken = default)
        {
            var video =
                await _videoRepository
                    .GetByIdAsync(
                        id,
                        cancellationToken);

            if (video is null)
            {
                return null;
            }

            return MapToDto(
                video);
        }


        // =========================================
        // CREATE
        // =========================================

        public async Task<VideoDto>
            CreateAsync(
                CreateVideoRequest request,
                CancellationToken cancellationToken = default)
        {
            if (request is null)
            {
                throw new ArgumentNullException(
                    nameof(request));
            }


            var title =
                request.Title?.Trim()
                ?? string.Empty;


            if (string.IsNullOrWhiteSpace(
                title))
            {
                throw new ArgumentException(
                    "Video title is required.");
            }


            if (title.Length > 200)
            {
                throw new ArgumentException(
                    "Video title cannot exceed 200 characters.");
            }


            var description =
                string.IsNullOrWhiteSpace(
                    request.Description)
                    ? null
                    : request.Description.Trim();


            if (
                description is not null &&
                description.Length > 1000
            )
            {
                throw new ArgumentException(
                    "Video description cannot exceed 1000 characters.");
            }


            ValidateSourceType(
                request.SourceType);


            var videoPath =
                NormalizeNullable(
                    request.VideoPath);


            var videoUrl =
                NormalizeNullable(
                    request.VideoUrl);


            ValidateVideoSource(
                request.SourceType,
                videoPath,
                videoUrl);


            var thumbnailPath =
                NormalizeNullable(
                    request.ThumbnailPath);


            // =========================================
            // YOUTUBE AUTO THUMBNAIL
            // =========================================
            // If admin does not upload a custom
            // thumbnail, automatically generate the
            // standard YouTube thumbnail URL.
            // =========================================

            if (
                request.SourceType ==
                    VideoSourceType.YouTube &&
                !request.HasCustomThumbnail
            )
            {
                thumbnailPath =
                    BuildYouTubeThumbnailUrl(
                        videoUrl!);
            }


            if (
                request.HasCustomThumbnail &&
                string.IsNullOrWhiteSpace(
                    thumbnailPath)
            )
            {
                throw new ArgumentException(
                    "Thumbnail path is required when a custom thumbnail is selected.");
            }


            if (request.DisplayOrder < 0)
            {
                throw new ArgumentException(
                    "Display order cannot be negative.");
            }


            var video =
                new Video
                {
                    Title =
                        title,

                    Description =
                        description,

                    SourceType =
                        request.SourceType,

                    VideoPath =
                        request.SourceType ==
                        VideoSourceType.UploadedFile
                            ? videoPath
                            : null,

                    VideoUrl =
                        request.SourceType ==
                        VideoSourceType.YouTube
                            ? videoUrl
                            : null,

                    ThumbnailPath =
                        thumbnailPath,

                    HasCustomThumbnail =
                        request.HasCustomThumbnail,

                    DisplayOrder =
                        request.DisplayOrder,

                    IsActive =
                        request.IsActive,

                    IsDeleted =
                        false
                };


            await _videoRepository
                .AddAsync(
                    video,
                    cancellationToken);


            await _videoRepository
                .SaveChangesAsync(
                    cancellationToken);


            return MapToDto(
                video);
        }


        // =========================================
        // UPDATE
        // =========================================

        public async Task<VideoDto>
            UpdateAsync(
                Guid id,
                UpdateVideoRequest request,
                CancellationToken cancellationToken = default)
        {
            if (request is null)
            {
                throw new ArgumentNullException(
                    nameof(request));
            }


            var video =
                await _videoRepository
                    .GetByIdAsync(
                        id,
                        cancellationToken);


            if (video is null)
            {
                throw new KeyNotFoundException(
                    "Video was not found.");
            }


            var title =
                request.Title?.Trim()
                ?? string.Empty;


            if (string.IsNullOrWhiteSpace(
                title))
            {
                throw new ArgumentException(
                    "Video title is required.");
            }


            if (title.Length > 200)
            {
                throw new ArgumentException(
                    "Video title cannot exceed 200 characters.");
            }


            var description =
                string.IsNullOrWhiteSpace(
                    request.Description)
                    ? null
                    : request.Description.Trim();


            if (
                description is not null &&
                description.Length > 1000
            )
            {
                throw new ArgumentException(
                    "Video description cannot exceed 1000 characters.");
            }


            ValidateSourceType(
                request.SourceType);


            var videoPath =
                NormalizeNullable(
                    request.VideoPath);


            var videoUrl =
                NormalizeNullable(
                    request.VideoUrl);


            ValidateVideoSource(
                request.SourceType,
                videoPath,
                videoUrl);


            var thumbnailPath =
                NormalizeNullable(
                    request.ThumbnailPath);


            // =========================================
            // YOUTUBE AUTO THUMBNAIL
            // =========================================

            if (
                request.SourceType ==
                    VideoSourceType.YouTube &&
                !request.HasCustomThumbnail
            )
            {
                thumbnailPath =
                    BuildYouTubeThumbnailUrl(
                        videoUrl!);
            }


            if (
                request.HasCustomThumbnail &&
                string.IsNullOrWhiteSpace(
                    thumbnailPath)
            )
            {
                throw new ArgumentException(
                    "Thumbnail path is required when a custom thumbnail is selected.");
            }


            if (request.DisplayOrder < 0)
            {
                throw new ArgumentException(
                    "Display order cannot be negative.");
            }


            video.Title =
                title;

            video.Description =
                description;

            video.SourceType =
                request.SourceType;


            // =========================================
            // PREVENT SOURCE CONFLICT
            // =========================================

            if (
                request.SourceType ==
                VideoSourceType.UploadedFile
            )
            {
                video.VideoPath =
                    videoPath;

                video.VideoUrl =
                    null;
            }
            else
            {
                video.VideoPath =
                    null;

                video.VideoUrl =
                    videoUrl;
            }


            video.ThumbnailPath =
                thumbnailPath;

            video.HasCustomThumbnail =
                request.HasCustomThumbnail;

            video.DisplayOrder =
                request.DisplayOrder;

            video.IsActive =
                request.IsActive;


            await _videoRepository
                .SaveChangesAsync(
                    cancellationToken);


            return MapToDto(
                video);
        }


        // =========================================
        // SET ACTIVE STATUS
        // =========================================

        public async Task SetActiveStatusAsync(
            Guid id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var video =
                await _videoRepository
                    .GetByIdAsync(
                        id,
                        cancellationToken);


            if (video is null)
            {
                throw new KeyNotFoundException(
                    "Video was not found.");
            }


            video.IsActive =
                isActive;


            await _videoRepository
                .SaveChangesAsync(
                    cancellationToken);
        }


        // =========================================
        // SOFT DELETE
        // =========================================

        public async Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var video =
                await _videoRepository
                    .GetByIdAsync(
                        id,
                        cancellationToken);


            if (video is null)
            {
                throw new KeyNotFoundException(
                    "Video was not found.");
            }


            video.IsDeleted =
                true;

            video.IsActive =
                false;


            await _videoRepository
                .SaveChangesAsync(
                    cancellationToken);
        }


        // =========================================
        // VALIDATE SOURCE TYPE
        // =========================================

        private static void ValidateSourceType(
            VideoSourceType sourceType)
        {
            if (
                sourceType !=
                    VideoSourceType.UploadedFile &&
                sourceType !=
                    VideoSourceType.YouTube
            )
            {
                throw new ArgumentException(
                    "Please select a valid video source type.");
            }
        }


        // =========================================
        // VALIDATE VIDEO SOURCE
        // =========================================

        private static void ValidateVideoSource(
            VideoSourceType sourceType,
            string? videoPath,
            string? videoUrl)
        {
            if (
                sourceType ==
                VideoSourceType.UploadedFile
            )
            {
                if (
                    string.IsNullOrWhiteSpace(
                        videoPath)
                )
                {
                    throw new ArgumentException(
                        "Uploaded video path is required.");
                }


                if (
                    !string.IsNullOrWhiteSpace(
                        videoUrl)
                )
                {
                    throw new ArgumentException(
                        "YouTube URL must be empty when the video source is Upload MP4.");
                }

                return;
            }


            if (
                sourceType ==
                VideoSourceType.YouTube
            )
            {
                if (
                    string.IsNullOrWhiteSpace(
                        videoUrl)
                )
                {
                    throw new ArgumentException(
                        "YouTube URL is required.");
                }


                if (
                    !string.IsNullOrWhiteSpace(
                        videoPath)
                )
                {
                    throw new ArgumentException(
                        "Uploaded video path must be empty when the video source is YouTube.");
                }


                if (
                    ExtractYouTubeVideoId(
                        videoUrl) is null
                )
                {
                    throw new ArgumentException(
                        "Please enter a valid YouTube video URL.");
                }
            }
        }


        // =========================================
        // YOUTUBE THUMBNAIL
        // =========================================

        private static string
            BuildYouTubeThumbnailUrl(
                string videoUrl)
        {
            var videoId =
                ExtractYouTubeVideoId(
                    videoUrl);


            if (
                string.IsNullOrWhiteSpace(
                    videoId)
            )
            {
                throw new ArgumentException(
                    "Unable to determine the YouTube video ID.");
            }


            return
                $"https://img.youtube.com/vi/{videoId}/hqdefault.jpg";
        }


        // =========================================
        // EXTRACT YOUTUBE VIDEO ID
        // =========================================
        // Supports:
        // youtube.com/watch?v=...
        // youtu.be/...
        // youtube.com/shorts/...
        // youtube.com/embed/...
        // =========================================

        private static string?
            ExtractYouTubeVideoId(
                string? videoUrl)
        {
            if (
                string.IsNullOrWhiteSpace(
                    videoUrl)
            )
            {
                return null;
            }


            if (
                !Uri.TryCreate(
                    videoUrl.Trim(),
                    UriKind.Absolute,
                    out var uri)
            )
            {
                return null;
            }


            var host =
                uri.Host
                    .ToLowerInvariant();


            if (
                host == "youtu.be" ||
                host == "www.youtu.be"
            )
            {
                var id =
                    uri.AbsolutePath
                        .Trim('/');

                return string.IsNullOrWhiteSpace(
                    id)
                    ? null
                    : id.Split('/')[0];
            }


            if (
                host != "youtube.com" &&
                host != "www.youtube.com" &&
                host != "m.youtube.com"
            )
            {
                return null;
            }


            var path =
                uri.AbsolutePath
                    .Trim('/');


            if (
                path.Equals(
                    "watch",
                    StringComparison.OrdinalIgnoreCase)
            )
            {
                var query =
                    uri.Query
                        .TrimStart('?')
                        .Split(
                            '&',
                            StringSplitOptions.RemoveEmptyEntries);


                foreach (
                    var item in query
                )
                {
                    var parts =
                        item.Split(
                            '=',
                            2);


                    if (
                        parts.Length == 2 &&
                        parts[0].Equals(
                            "v",
                            StringComparison.OrdinalIgnoreCase)
                    )
                    {
                        return Uri.UnescapeDataString(
                            parts[1]);
                    }
                }

                return null;
            }


            var segments =
                path.Split(
                    '/',
                    StringSplitOptions.RemoveEmptyEntries);


            if (
                segments.Length >= 2 &&
                (
                    segments[0].Equals(
                        "shorts",
                        StringComparison.OrdinalIgnoreCase) ||
                    segments[0].Equals(
                        "embed",
                        StringComparison.OrdinalIgnoreCase)
                )
            )
            {
                return segments[1];
            }


            return null;
        }


        // =========================================
        // NORMALIZE NULLABLE STRING
        // =========================================

        private static string?
            NormalizeNullable(
                string? value)
        {
            return string.IsNullOrWhiteSpace(
                value)
                ? null
                : value.Trim();
        }


        // =========================================
        // MAP ENTITY TO DTO
        // =========================================

        private static VideoDto
            MapToDto(
                Video video)
        {
            return new VideoDto
            {
                Id =
                    video.Id,

                Title =
                    video.Title,

                Description =
                    video.Description,

                SourceType =
                    video.SourceType,

                VideoPath =
                    video.VideoPath,

                VideoUrl =
                    video.VideoUrl,

                ThumbnailPath =
                    video.ThumbnailPath,

                HasCustomThumbnail =
                    video.HasCustomThumbnail,

                DisplayOrder =
                    video.DisplayOrder,

                IsActive =
                    video.IsActive,

                CreatedAt =
                    video.CreatedAt,

                UpdatedAt =
                    video.UpdatedAt
            };
        }
    }
}