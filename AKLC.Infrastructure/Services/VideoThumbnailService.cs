using AKLC.Application.Interfaces;

using Microsoft.AspNetCore.Hosting;

using System.Diagnostics;

namespace AKLC.Infrastructure.Services
{
    public class VideoThumbnailService
        : IVideoThumbnailService
    {
        private readonly IWebHostEnvironment
            _environment;


        // =========================================
        // CONSTRUCTOR
        // =========================================

        public VideoThumbnailService(
            IWebHostEnvironment environment)
        {
            _environment =
                environment;
        }


        // =========================================
        // GENERATE THUMBNAIL
        // =========================================

        public async Task<string?>
            GenerateThumbnailAsync(
                string videoPath,
                CancellationToken cancellationToken = default)
        {
            if (
                string.IsNullOrWhiteSpace(
                    videoPath)
            )
            {
                return null;
            }


            // =========================================
            // WEB ROOT
            // =========================================

            var webRoot =
                _environment.WebRootPath;


            if (
                string.IsNullOrWhiteSpace(
                    webRoot)
            )
            {
                webRoot =
                    Path.Combine(
                        _environment.ContentRootPath,
                        "wwwroot");
            }


            // =========================================
            // SOURCE VIDEO PATH
            // =========================================

            var relativeVideoPath =
                videoPath
                    .Trim()
                    .TrimStart('/')
                    .Replace(
                        '/',
                        Path.DirectorySeparatorChar);


            var sourceVideoPath =
                Path.Combine(
                    webRoot,
                    relativeVideoPath);


            if (
                !File.Exists(
                    sourceVideoPath)
            )
            {
                throw new FileNotFoundException(
                    "Uploaded video file was not found.",
                    sourceVideoPath);
            }


            // =========================================
            // THUMBNAIL FOLDER
            // =========================================

            const string thumbnailFolder =
                "uploads/videos/thumbnails";


            var thumbnailFolderPath =
                Path.Combine(
                    webRoot,
                    thumbnailFolder.Replace(
                        '/',
                        Path.DirectorySeparatorChar));


            Directory.CreateDirectory(
                thumbnailFolderPath);


            // =========================================
            // GENERATED FILE NAME
            // =========================================

            var generatedFileName =
                $"{Guid.NewGuid():N}.jpg";


            var outputPath =
                Path.Combine(
                    thumbnailFolderPath,
                    generatedFileName);


            // =========================================
            // FFMPEG
            // =========================================
            //
            // Captures one frame approximately
            // one second into the video.
            //
            // FFmpeg must be installed on the
            // development / IIS server and available
            // through the system PATH.
            // =========================================

            var startInfo =
                new ProcessStartInfo
                {
                    FileName =
                        "ffmpeg",

                    RedirectStandardOutput =
                        true,

                    RedirectStandardError =
                        true,

                    UseShellExecute =
                        false,

                    CreateNoWindow =
                        true
                };


            startInfo.ArgumentList.Add(
                "-y");

            startInfo.ArgumentList.Add(
                "-ss");

            startInfo.ArgumentList.Add(
                "00:00:01");

            startInfo.ArgumentList.Add(
                "-i");

            startInfo.ArgumentList.Add(
                sourceVideoPath);

            startInfo.ArgumentList.Add(
                "-frames:v");

            startInfo.ArgumentList.Add(
                "1");

            startInfo.ArgumentList.Add(
                "-q:v");

            startInfo.ArgumentList.Add(
                "2");

            startInfo.ArgumentList.Add(
                outputPath);


            using var process =
                new Process
                {
                    StartInfo =
                        startInfo
                };


            try
            {
                process.Start();
            }
            catch (
                Exception exception
            )
            {
                throw new InvalidOperationException(
                    "FFmpeg could not be started. Please make sure FFmpeg is installed and available in the system PATH.",
                    exception);
            }


            var errorTask =
                process.StandardError
                    .ReadToEndAsync(
                        cancellationToken);


            await process.WaitForExitAsync(
                cancellationToken);


            var errorOutput =
                await errorTask;


            if (
                process.ExitCode != 0 ||
                !File.Exists(
                    outputPath)
            )
            {
                if (
                    File.Exists(
                        outputPath)
                )
                {
                    File.Delete(
                        outputPath);
                }


                throw new InvalidOperationException(
                    $"Unable to generate video thumbnail. FFmpeg returned exit code {process.ExitCode}. {errorOutput}");
            }


            // =========================================
            // RETURN PUBLIC PATH
            // =========================================

            return
                $"/uploads/videos/thumbnails/{generatedFileName}";
        }
    }
}