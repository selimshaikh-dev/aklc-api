using AKLC.Api.Models.Videos;
using AKLC.Application.DTOs.Videos;
using AKLC.Application.Interfaces;
using AKLC.Domain.Constants;
using AKLC.Domain.Entities;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AKLC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Admin)]
    public class VideosController
        : ControllerBase
    {
        private readonly IVideoService
            _videoService;

        private readonly IFileStorageService
            _fileStorageService;

        private readonly IVideoThumbnailService
            _videoThumbnailService;


        // =========================================
        // FILE LIMITS
        // =========================================

        private const long MaxVideoFileSize =
            500L * 1024L * 1024L;

        private const long MaxThumbnailFileSize =
            10L * 1024L * 1024L;


        // =========================================
        // CONSTRUCTOR
        // =========================================

        public VideosController(
            IVideoService videoService,
            IFileStorageService fileStorageService,
            IVideoThumbnailService videoThumbnailService)
        {
            _videoService =
                videoService;

            _fileStorageService =
                fileStorageService;

            _videoThumbnailService =
                videoThumbnailService;
        }


        // =========================================
        // PUBLIC ACTIVE VIDEOS
        // GET: api/Videos/public
        // =========================================

        [AllowAnonymous]
        [HttpGet("public")]
        public async Task<
            ActionResult<IReadOnlyList<VideoDto>>>
            GetPublicVideos(
                CancellationToken cancellationToken)
        {
            var videos =
                await _videoService
                    .GetAllActiveAsync(
                        cancellationToken);

            return Ok(
                videos);
        }


        // =========================================
        // ADMIN - GET ALL VIDEOS
        // GET: api/Videos
        // =========================================

        [HttpGet]
        public async Task<
            ActionResult<IReadOnlyList<VideoDto>>>
            GetAll(
                CancellationToken cancellationToken)
        {
            var videos =
                await _videoService
                    .GetAllAsync(
                        cancellationToken);

            return Ok(
                videos);
        }


        // =========================================
        // ADMIN - GET VIDEO BY ID
        // GET: api/Videos/{id}
        // =========================================

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<VideoDto>>
            GetById(
                Guid id,
                CancellationToken cancellationToken)
        {
            var video =
                await _videoService
                    .GetByIdAsync(
                        id,
                        cancellationToken);


            if (video is null)
            {
                return NotFound(
                    new
                    {
                        message =
                            "Video was not found."
                    });
            }


            return Ok(
                video);
        }


        // =========================================
        // ADMIN - CREATE VIDEO
        // POST: api/Videos
        // Content-Type: multipart/form-data
        // =========================================

        [HttpPost]
        [RequestSizeLimit(MaxVideoFileSize + MaxThumbnailFileSize)]
        [RequestFormLimits(
            MultipartBodyLengthLimit =
                MaxVideoFileSize +
                MaxThumbnailFileSize)]
        public async Task<ActionResult<VideoDto>>
            Create(
                [FromForm]
                CreateVideoForm form,
                CancellationToken cancellationToken)
        {
            string? savedVideoPath =
                null;

            string? savedThumbnailPath =
                null;


            try
            {
                // =====================================
                // SOURCE TYPE
                // =====================================

                if (
                    form.SourceType !=
                        VideoSourceType.UploadedFile &&
                    form.SourceType !=
                        VideoSourceType.YouTube
                )
                {
                    return BadRequest(
                        new
                        {
                            message =
                                "Please select a valid video source type."
                        });
                }


                // =====================================
                // UPLOADED MP4
                // =====================================

                if (
                    form.SourceType ==
                    VideoSourceType.UploadedFile
                )
                {
                    if (form.VideoFile is null)
                    {
                        return BadRequest(
                            new
                            {
                                message =
                                    "Please select an MP4 video file."
                            });
                    }


                    if (
                        !string.IsNullOrWhiteSpace(
                            form.VideoUrl)
                    )
                    {
                        return BadRequest(
                            new
                            {
                                message =
                                    "YouTube URL must be empty when uploading an MP4 video."
                            });
                    }


                    var videoValidationError =
                        ValidateVideoFile(
                            form.VideoFile);


                    if (
                        videoValidationError is not null
                    )
                    {
                        return BadRequest(
                            new
                            {
                                message =
                                    videoValidationError
                            });
                    }


                    await using (
                        var videoStream =
                            form.VideoFile
                                .OpenReadStream()
                    )
                    {
                        savedVideoPath =
                            await _fileStorageService
                                .SaveFileAsync(
                                    videoStream,
                                    form.VideoFile.FileName,
                                    "uploads/videos",
                                    cancellationToken);
                    }


                    if (
                        string.IsNullOrWhiteSpace(
                            savedVideoPath)
                    )
                    {
                        throw new InvalidOperationException(
                            "The video file could not be saved.");
                    }
                }


                // =====================================
                // YOUTUBE
                // =====================================

                if (
                    form.SourceType ==
                    VideoSourceType.YouTube
                )
                {
                    if (form.VideoFile is not null)
                    {
                        return BadRequest(
                            new
                            {
                                message =
                                    "MP4 upload must be empty when the video source is YouTube."
                            });
                    }


                    if (
                        string.IsNullOrWhiteSpace(
                            form.VideoUrl)
                    )
                    {
                        return BadRequest(
                            new
                            {
                                message =
                                    "YouTube URL is required."
                            });
                    }
                }


                // =====================================
                // CUSTOM THUMBNAIL
                // =====================================

                var hasCustomThumbnail =
                    form.ThumbnailFile is not null;


                if (
                    form.ThumbnailFile is not null
                )
                {
                    var thumbnailValidationError =
                        ValidateThumbnailFile(
                            form.ThumbnailFile);


                    if (
                        thumbnailValidationError
                            is not null
                    )
                    {
                        return BadRequest(
                            new
                            {
                                message =
                                    thumbnailValidationError
                            });
                    }


                    await using (
                        var thumbnailStream =
                            form.ThumbnailFile
                                .OpenReadStream()
                    )
                    {
                        savedThumbnailPath =
                            await _fileStorageService
                                .SaveFileAsync(
                                    thumbnailStream,
                                    form.ThumbnailFile.FileName,
                                    "uploads/videos/thumbnails",
                                    cancellationToken);
                    }


                    if (
                        string.IsNullOrWhiteSpace(
                            savedThumbnailPath)
                    )
                    {
                        throw new InvalidOperationException(
                            "The thumbnail file could not be saved.");
                    }
                }


                // =====================================
                // MP4 AUTO THUMBNAIL
                // =====================================

                if (
                    form.SourceType ==
                        VideoSourceType.UploadedFile &&
                    !hasCustomThumbnail
                )
                {
                    savedThumbnailPath =
                        await _videoThumbnailService
                            .GenerateThumbnailAsync(
                                savedVideoPath!,
                                cancellationToken);


                    if (
                        string.IsNullOrWhiteSpace(
                            savedThumbnailPath)
                    )
                    {
                        throw new InvalidOperationException(
                            "The video thumbnail could not be generated.");
                    }
                }


                // =====================================
                // APPLICATION REQUEST
                // =====================================

                var request =
                    new CreateVideoRequest
                    {
                        Title =
                            form.Title,

                        Description =
                            form.Description,

                        SourceType =
                            form.SourceType,

                        VideoPath =
                            form.SourceType ==
                            VideoSourceType.UploadedFile
                                ? savedVideoPath
                                : null,

                        VideoUrl =
                            form.SourceType ==
                            VideoSourceType.YouTube
                                ? form.VideoUrl
                                : null,

                        ThumbnailPath =
                            savedThumbnailPath,

                        HasCustomThumbnail =
                            hasCustomThumbnail,

                        DisplayOrder =
                            form.DisplayOrder,

                        IsActive =
                            form.IsActive
                    };


                var video =
                    await _videoService
                        .CreateAsync(
                            request,
                            cancellationToken);


                return Created(
                    $"/api/Videos/{video.Id}",
                    video);
            }
            catch (
                ArgumentException exception
            )
            {
                await DeleteLocalFileSafelyAsync(
                    savedVideoPath,
                    cancellationToken);

                await DeleteLocalFileSafelyAsync(
                    savedThumbnailPath,
                    cancellationToken);


                return BadRequest(
                    new
                    {
                        message =
                            exception.Message
                    });
            }
            catch
            {
                await DeleteLocalFileSafelyAsync(
                    savedVideoPath,
                    cancellationToken);

                await DeleteLocalFileSafelyAsync(
                    savedThumbnailPath,
                    cancellationToken);

                throw;
            }
        }


        // =========================================
        // ADMIN - UPDATE VIDEO
        // PUT: api/Videos/{id}
        // Content-Type: multipart/form-data
        // =========================================

        [HttpPut("{id:guid}")]
        [RequestSizeLimit(MaxVideoFileSize + MaxThumbnailFileSize)]
        [RequestFormLimits(
            MultipartBodyLengthLimit =
                MaxVideoFileSize +
                MaxThumbnailFileSize)]
        public async Task<ActionResult<VideoDto>>
            Update(
                Guid id,
                [FromForm]
                UpdateVideoForm form,
                CancellationToken cancellationToken)
        {
            var existingVideo =
                await _videoService
                    .GetByIdAsync(
                        id,
                        cancellationToken);


            if (existingVideo is null)
            {
                return NotFound(
                    new
                    {
                        message =
                            "Video was not found."
                    });
            }


            string? newVideoPath =
                null;

            string? newThumbnailPath =
                null;


            try
            {
                if (
                    form.SourceType !=
                        VideoSourceType.UploadedFile &&
                    form.SourceType !=
                        VideoSourceType.YouTube
                )
                {
                    return BadRequest(
                        new
                        {
                            message =
                                "Please select a valid video source type."
                        });
                }


                string? finalVideoPath =
                    null;

                string? finalVideoUrl =
                    null;

                string? finalThumbnailPath =
                    null;

                bool finalHasCustomThumbnail =
                    false;


                // =====================================
                // UPLOADED MP4 SOURCE
                // =====================================

                if (
                    form.SourceType ==
                    VideoSourceType.UploadedFile
                )
                {
                    if (
                        !string.IsNullOrWhiteSpace(
                            form.VideoUrl)
                    )
                    {
                        return BadRequest(
                            new
                            {
                                message =
                                    "YouTube URL must be empty when the video source is Upload MP4."
                            });
                    }


                    // ---------------------------------
                    // NEW MP4
                    // ---------------------------------

                    if (
                        form.VideoFile is not null
                    )
                    {
                        var videoValidationError =
                            ValidateVideoFile(
                                form.VideoFile);


                        if (
                            videoValidationError
                                is not null
                        )
                        {
                            return BadRequest(
                                new
                                {
                                    message =
                                        videoValidationError
                                });
                        }


                        await using (
                            var videoStream =
                                form.VideoFile
                                    .OpenReadStream()
                        )
                        {
                            newVideoPath =
                                await _fileStorageService
                                    .SaveFileAsync(
                                        videoStream,
                                        form.VideoFile.FileName,
                                        "uploads/videos",
                                        cancellationToken);
                        }


                        if (
                            string.IsNullOrWhiteSpace(
                                newVideoPath)
                        )
                        {
                            throw new InvalidOperationException(
                                "The video file could not be saved.");
                        }


                        finalVideoPath =
                            newVideoPath;
                    }
                    else
                    {
                        // Existing MP4 may remain.
                        if (
                            existingVideo.SourceType ==
                                VideoSourceType.UploadedFile &&
                            !string.IsNullOrWhiteSpace(
                                existingVideo.VideoPath)
                        )
                        {
                            finalVideoPath =
                                existingVideo.VideoPath;
                        }
                        else
                        {
                            return BadRequest(
                                new
                                {
                                    message =
                                        "Please upload an MP4 video when changing the source to Upload MP4."
                                });
                        }
                    }


                    // =================================
                    // CUSTOM THUMBNAIL UPLOAD
                    // =================================

                    if (
                        form.ThumbnailFile is not null
                    )
                    {
                        var thumbnailValidationError =
                            ValidateThumbnailFile(
                                form.ThumbnailFile);


                        if (
                            thumbnailValidationError
                                is not null
                        )
                        {
                            return BadRequest(
                                new
                                {
                                    message =
                                        thumbnailValidationError
                                });
                        }


                        await using (
                            var thumbnailStream =
                                form.ThumbnailFile
                                    .OpenReadStream()
                        )
                        {
                            newThumbnailPath =
                                await _fileStorageService
                                    .SaveFileAsync(
                                        thumbnailStream,
                                        form.ThumbnailFile.FileName,
                                        "uploads/videos/thumbnails",
                                        cancellationToken);
                        }


                        if (
                            string.IsNullOrWhiteSpace(
                                newThumbnailPath)
                        )
                        {
                            throw new InvalidOperationException(
                                "The thumbnail file could not be saved.");
                        }


                        finalThumbnailPath =
                            newThumbnailPath;

                        finalHasCustomThumbnail =
                            true;
                    }

                    // =================================
                    // REMOVE CUSTOM THUMBNAIL
                    // =================================

                    else if (
                        form.RemoveCustomThumbnail
                    )
                    {
                        newThumbnailPath =
                            await _videoThumbnailService
                                .GenerateThumbnailAsync(
                                    finalVideoPath!,
                                    cancellationToken);


                        finalThumbnailPath =
                            newThumbnailPath;

                        finalHasCustomThumbnail =
                            false;
                    }

                    // =================================
                    // NEW MP4 → NEW AUTO THUMBNAIL
                    // =================================

                    else if (
                        newVideoPath is not null
                    )
                    {
                        newThumbnailPath =
                            await _videoThumbnailService
                                .GenerateThumbnailAsync(
                                    finalVideoPath!,
                                    cancellationToken);


                        finalThumbnailPath =
                            newThumbnailPath;

                        finalHasCustomThumbnail =
                            false;
                    }

                    // =================================
                    // KEEP EXISTING THUMBNAIL
                    // =================================

                    else if (
                        existingVideo.SourceType ==
                        VideoSourceType.UploadedFile
                    )
                    {
                        finalThumbnailPath =
                            existingVideo.ThumbnailPath;

                        finalHasCustomThumbnail =
                            existingVideo.HasCustomThumbnail;


                        if (
                            string.IsNullOrWhiteSpace(
                                finalThumbnailPath)
                        )
                        {
                            newThumbnailPath =
                                await _videoThumbnailService
                                    .GenerateThumbnailAsync(
                                        finalVideoPath!,
                                        cancellationToken);

                            finalThumbnailPath =
                                newThumbnailPath;

                            finalHasCustomThumbnail =
                                false;
                        }
                    }

                    // =================================
                    // SOURCE CHANGED TO MP4
                    // =================================

                    else
                    {
                        newThumbnailPath =
                            await _videoThumbnailService
                                .GenerateThumbnailAsync(
                                    finalVideoPath!,
                                    cancellationToken);


                        finalThumbnailPath =
                            newThumbnailPath;

                        finalHasCustomThumbnail =
                            false;
                    }
                }


                // =====================================
                // YOUTUBE SOURCE
                // =====================================

                if (
                    form.SourceType ==
                    VideoSourceType.YouTube
                )
                {
                    if (
                        form.VideoFile is not null
                    )
                    {
                        return BadRequest(
                            new
                            {
                                message =
                                    "MP4 upload must be empty when the video source is YouTube."
                            });
                    }


                    if (
                        string.IsNullOrWhiteSpace(
                            form.VideoUrl)
                    )
                    {
                        return BadRequest(
                            new
                            {
                                message =
                                    "YouTube URL is required."
                            });
                    }


                    finalVideoUrl =
                        form.VideoUrl;


                    // =================================
                    // CUSTOM THUMBNAIL UPLOAD
                    // =================================

                    if (
                        form.ThumbnailFile is not null
                    )
                    {
                        var thumbnailValidationError =
                            ValidateThumbnailFile(
                                form.ThumbnailFile);


                        if (
                            thumbnailValidationError
                                is not null
                        )
                        {
                            return BadRequest(
                                new
                                {
                                    message =
                                        thumbnailValidationError
                                });
                        }


                        await using (
                            var thumbnailStream =
                                form.ThumbnailFile
                                    .OpenReadStream()
                        )
                        {
                            newThumbnailPath =
                                await _fileStorageService
                                    .SaveFileAsync(
                                        thumbnailStream,
                                        form.ThumbnailFile.FileName,
                                        "uploads/videos/thumbnails",
                                        cancellationToken);
                        }


                        if (
                            string.IsNullOrWhiteSpace(
                                newThumbnailPath)
                        )
                        {
                            throw new InvalidOperationException(
                                "The thumbnail file could not be saved.");
                        }


                        finalThumbnailPath =
                            newThumbnailPath;

                        finalHasCustomThumbnail =
                            true;
                    }

                    // =================================
                    // KEEP EXISTING CUSTOM THUMBNAIL
                    // =================================

                    else if (
                        !form.RemoveCustomThumbnail &&
                        existingVideo.SourceType ==
                            VideoSourceType.YouTube &&
                        existingVideo.HasCustomThumbnail &&
                        !string.IsNullOrWhiteSpace(
                            existingVideo.ThumbnailPath)
                    )
                    {
                        finalThumbnailPath =
                            existingVideo.ThumbnailPath;

                        finalHasCustomThumbnail =
                            true;
                    }

                    // =================================
                    // YOUTUBE AUTO THUMBNAIL
                    // =================================

                    else
                    {
                        finalThumbnailPath =
                            null;

                        finalHasCustomThumbnail =
                            false;
                    }
                }


                // =====================================
                // APPLICATION REQUEST
                // =====================================

                var request =
                    new UpdateVideoRequest
                    {
                        Title =
                            form.Title,

                        Description =
                            form.Description,

                        SourceType =
                            form.SourceType,

                        VideoPath =
                            finalVideoPath,

                        VideoUrl =
                            finalVideoUrl,

                        ThumbnailPath =
                            finalThumbnailPath,

                        HasCustomThumbnail =
                            finalHasCustomThumbnail,

                        DisplayOrder =
                            form.DisplayOrder,

                        IsActive =
                            form.IsActive
                    };


                var updatedVideo =
                    await _videoService
                        .UpdateAsync(
                            id,
                            request,
                            cancellationToken);


                // =====================================
                // DELETE REPLACED OLD VIDEO
                // =====================================

                if (
                    !string.IsNullOrWhiteSpace(
                        existingVideo.VideoPath) &&
                    !string.Equals(
                        existingVideo.VideoPath,
                        updatedVideo.VideoPath,
                        StringComparison.OrdinalIgnoreCase)
                )
                {
                    await DeleteLocalFileSafelyAsync(
                        existingVideo.VideoPath,
                        cancellationToken);
                }


                // =====================================
                // DELETE REPLACED OLD THUMBNAIL
                // =====================================

                if (
                    !string.IsNullOrWhiteSpace(
                        existingVideo.ThumbnailPath) &&
                    !string.Equals(
                        existingVideo.ThumbnailPath,
                        updatedVideo.ThumbnailPath,
                        StringComparison.OrdinalIgnoreCase)
                )
                {
                    await DeleteLocalFileSafelyAsync(
                        existingVideo.ThumbnailPath,
                        cancellationToken);
                }


                return Ok(
                    updatedVideo);
            }
            catch (
                ArgumentException exception
            )
            {
                await DeleteLocalFileSafelyAsync(
                    newVideoPath,
                    cancellationToken);

                await DeleteLocalFileSafelyAsync(
                    newThumbnailPath,
                    cancellationToken);


                return BadRequest(
                    new
                    {
                        message =
                            exception.Message
                    });
            }
            catch (
                KeyNotFoundException
            )
            {
                await DeleteLocalFileSafelyAsync(
                    newVideoPath,
                    cancellationToken);

                await DeleteLocalFileSafelyAsync(
                    newThumbnailPath,
                    cancellationToken);


                return NotFound(
                    new
                    {
                        message =
                            "Video was not found."
                    });
            }
            catch
            {
                await DeleteLocalFileSafelyAsync(
                    newVideoPath,
                    cancellationToken);

                await DeleteLocalFileSafelyAsync(
                    newThumbnailPath,
                    cancellationToken);

                throw;
            }
        }


        // =========================================
        // ADMIN - ACTIVE / INACTIVE
        // PATCH:
        // api/Videos/{id}/status?isActive=true
        // =========================================

        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult>
            SetActiveStatus(
                Guid id,
                [FromQuery]
                bool isActive,
                CancellationToken cancellationToken)
        {
            try
            {
                await _videoService
                    .SetActiveStatusAsync(
                        id,
                        isActive,
                        cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(
                    new
                    {
                        message =
                            "Video was not found."
                    });
            }
        }


        // =========================================
        // ADMIN - SOFT DELETE
        // DELETE: api/Videos/{id}
        // =========================================

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult>
            Delete(
                Guid id,
                CancellationToken cancellationToken)
        {
            try
            {
                await _videoService
                    .DeleteAsync(
                        id,
                        cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(
                    new
                    {
                        message =
                            "Video was not found."
                    });
            }
        }


        // =========================================
        // VALIDATE VIDEO FILE
        // =========================================

        private static string?
            ValidateVideoFile(
                IFormFile file)
        {
            if (file.Length <= 0)
            {
                return
                    "The selected video file is empty.";
            }


            if (
                file.Length >
                MaxVideoFileSize
            )
            {
                return
                    "The MP4 video cannot exceed 500 MB.";
            }


            var extension =
                Path.GetExtension(
                    file.FileName)
                    .ToLowerInvariant();


            if (
                extension != ".mp4"
            )
            {
                return
                    "Only MP4 video files are allowed.";
            }


            return null;
        }


        // =========================================
        // VALIDATE THUMBNAIL FILE
        // =========================================

        private static string?
            ValidateThumbnailFile(
                IFormFile file)
        {
            if (file.Length <= 0)
            {
                return
                    "The selected thumbnail file is empty.";
            }


            if (
                file.Length >
                MaxThumbnailFileSize
            )
            {
                return
                    "The thumbnail image cannot exceed 10 MB.";
            }


            var extension =
                Path.GetExtension(
                    file.FileName)
                    .ToLowerInvariant();


            var allowedExtensions =
                new[]
                {
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".webp"
                };


            if (
                !allowedExtensions.Contains(
                    extension)
            )
            {
                return
                    "Thumbnail must be JPG, JPEG, PNG, or WEBP.";
            }


            return null;
        }


        // =========================================
        // DELETE LOCAL FILE SAFELY
        // =========================================

        private async Task
            DeleteLocalFileSafelyAsync(
                string? filePath,
                CancellationToken cancellationToken)
        {
            if (
                string.IsNullOrWhiteSpace(
                    filePath)
            )
            {
                return;
            }


            // Do not try to delete remote URLs,
            // such as YouTube thumbnail URLs.
            if (
                filePath.StartsWith(
                    "http://",
                    StringComparison.OrdinalIgnoreCase) ||
                filePath.StartsWith(
                    "https://",
                    StringComparison.OrdinalIgnoreCase)
            )
            {
                return;
            }


            await _fileStorageService
                .DeleteFileAsync(
                    filePath,
                    cancellationToken);
        }
    }
}