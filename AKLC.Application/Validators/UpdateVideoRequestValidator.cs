using AKLC.Application.DTOs.Videos;
using AKLC.Domain.Entities;
using FluentValidation;

namespace AKLC.Application.Validators
{
    public class UpdateVideoRequestValidator
        : AbstractValidator<UpdateVideoRequest>
    {
        public UpdateVideoRequestValidator()
        {
            // =========================================
            // TITLE
            // =========================================

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Video title is required.")
                .MaximumLength(200)
                .WithMessage(
                    "Video title cannot exceed 200 characters.");


            // =========================================
            // DESCRIPTION
            // =========================================

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage(
                    "Video description cannot exceed 1000 characters.")
                .When(x =>
                    !string.IsNullOrWhiteSpace(
                        x.Description));


            // =========================================
            // SOURCE TYPE
            // =========================================

            RuleFor(x => x.SourceType)
                .IsInEnum()
                .WithMessage(
                    "Please select a valid video source type.");


            // =========================================
            // UPLOADED MP4
            // =========================================

            RuleFor(x => x.VideoPath)
                .NotEmpty()
                .WithMessage(
                    "Uploaded video path is required when the video source is Upload MP4.")
                .When(x =>
                    x.SourceType ==
                    VideoSourceType.UploadedFile);


            RuleFor(x => x.VideoPath)
                .MaximumLength(500)
                .WithMessage(
                    "Video path cannot exceed 500 characters.")
                .When(x =>
                    !string.IsNullOrWhiteSpace(
                        x.VideoPath));


            RuleFor(x => x.VideoUrl)
                .Must(string.IsNullOrWhiteSpace)
                .WithMessage(
                    "YouTube URL must be empty when the video source is Upload MP4.")
                .When(x =>
                    x.SourceType ==
                    VideoSourceType.UploadedFile);


            // =========================================
            // YOUTUBE VIDEO
            // =========================================

            RuleFor(x => x.VideoUrl)
                .NotEmpty()
                .WithMessage(
                    "YouTube URL is required when the video source is YouTube.")
                .When(x =>
                    x.SourceType ==
                    VideoSourceType.YouTube);


            RuleFor(x => x.VideoUrl)
                .MaximumLength(1000)
                .WithMessage(
                    "Video URL cannot exceed 1000 characters.")
                .When(x =>
                    !string.IsNullOrWhiteSpace(
                        x.VideoUrl));


            RuleFor(x => x.VideoUrl)
                .Must(BeValidYouTubeUrl)
                .WithMessage(
                    "Please enter a valid YouTube video URL.")
                .When(x =>
                    x.SourceType ==
                        VideoSourceType.YouTube &&
                    !string.IsNullOrWhiteSpace(
                        x.VideoUrl));


            RuleFor(x => x.VideoPath)
                .Must(string.IsNullOrWhiteSpace)
                .WithMessage(
                    "Uploaded video path must be empty when the video source is YouTube.")
                .When(x =>
                    x.SourceType ==
                    VideoSourceType.YouTube);


            // =========================================
            // THUMBNAIL
            // =========================================

            RuleFor(x => x.ThumbnailPath)
                .MaximumLength(500)
                .WithMessage(
                    "Thumbnail path cannot exceed 500 characters.")
                .When(x =>
                    !string.IsNullOrWhiteSpace(
                        x.ThumbnailPath));


            RuleFor(x => x.ThumbnailPath)
                .NotEmpty()
                .WithMessage(
                    "Thumbnail path is required when a custom thumbnail is selected.")
                .When(x =>
                    x.HasCustomThumbnail);


            // =========================================
            // DISPLAY ORDER
            // =========================================

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0)
                .WithMessage(
                    "Display order cannot be negative.");
        }


        // =========================================
        // YOUTUBE URL VALIDATION
        // =========================================

        private static bool BeValidYouTubeUrl(
            string? url)
        {
            if (
                string.IsNullOrWhiteSpace(
                    url)
            )
            {
                return false;
            }


            if (
                !Uri.TryCreate(
                    url.Trim(),
                    UriKind.Absolute,
                    out var uri)
            )
            {
                return false;
            }


            if (
                uri.Scheme != Uri.UriSchemeHttp &&
                uri.Scheme != Uri.UriSchemeHttps
            )
            {
                return false;
            }


            var host =
                uri.Host
                    .ToLowerInvariant();


            return
                host == "youtube.com" ||
                host == "www.youtube.com" ||
                host == "m.youtube.com" ||
                host == "youtu.be" ||
                host == "www.youtu.be";
        }
    }
}