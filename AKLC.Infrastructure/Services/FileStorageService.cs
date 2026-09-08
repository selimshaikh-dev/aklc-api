using AKLC.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace AKLC.Infrastructure.Services;

public class FileStorageService
    : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;

    public FileStorageService(
        IWebHostEnvironment environment)
    {
        _environment = environment;
    }


    // =========================================
    // SAVE FILE
    // =========================================

    public async Task<string?> SaveFileAsync(
        Stream fileStream,
        string fileName,
        string folder,
        CancellationToken cancellationToken = default)
    {
        if (fileStream is null)
        {
            return null;
        }

        var webRoot =
            _environment.WebRootPath;

        if (string.IsNullOrWhiteSpace(webRoot))
        {
            webRoot =
                Path.Combine(
                    _environment.ContentRootPath,
                    "wwwroot");
        }


        var folderPath =
            Path.Combine(
                webRoot,
                folder.Replace(
                    '/',
                    Path.DirectorySeparatorChar));


        Directory.CreateDirectory(
            folderPath);


        var extension =
            Path.GetExtension(
                fileName)
                .ToLowerInvariant();


        var generatedFileName =
            $"{Guid.NewGuid():N}{extension}";


        var fullPath =
            Path.Combine(
                folderPath,
                generatedFileName);


        await using var outputStream =
            new FileStream(
                fullPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                81920,
                useAsync: true);


        await fileStream.CopyToAsync(
            outputStream,
            cancellationToken);


        var normalizedFolder =
            folder
                .Replace("\\", "/")
                .Trim('/');


        return
            $"/{normalizedFolder}/{generatedFileName}";
    }


    // =========================================
    // DELETE FILE
    // =========================================

    public Task DeleteFileAsync(
        string? filePath,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(
            filePath))
        {
            return Task.CompletedTask;
        }


        var webRoot =
            _environment.WebRootPath;

        if (string.IsNullOrWhiteSpace(webRoot))
        {
            webRoot =
                Path.Combine(
                    _environment.ContentRootPath,
                    "wwwroot");
        }


        var relativePath =
            filePath
                .Trim()
                .TrimStart('/')
                .Replace(
                    '/',
                    Path.DirectorySeparatorChar);


        var fullPath =
            Path.Combine(
                webRoot,
                relativePath);


        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }


        return Task.CompletedTask;
    }
}