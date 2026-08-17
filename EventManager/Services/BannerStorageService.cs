using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace EventManager.Services;

public sealed class BannerStorageService(
    IConfiguration configuration,
    IWebHostEnvironment environment) : IBannerStorageService
{
    private const long MaxFileSize = 5 * 1024 * 1024;

    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    private static readonly HashSet<string> AllowedContentTypes =
        new(StringComparer.OrdinalIgnoreCase) { "image/jpeg", "image/png", "image/gif", "image/webp" };

    public async Task<string> SaveAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        Validate(file);

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var blobName = $"{Guid.NewGuid():N}{extension}";
        var connectionString = configuration["AzureBlobStorage:ConnectionString"];

        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            return await UploadToAzureAsync(file, blobName, connectionString, cancellationToken);
        }

        if (!environment.IsDevelopment())
        {
            throw new InvalidOperationException(
                "Azure Blob Storage is not configured. Set AzureBlobStorage__ConnectionString.");
        }

        return await SaveLocallyAsync(file, blobName, cancellationToken);
    }

    private async Task<string> UploadToAzureAsync(
        IFormFile file,
        string blobName,
        string connectionString,
        CancellationToken cancellationToken)
    {
        var containerName = configuration["AzureBlobStorage:ContainerName"] ?? "event-banners";
        var container = new BlobContainerClient(connectionString, containerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);

        var blob = container.GetBlobClient(blobName);
        await using var stream = file.OpenReadStream();
        await blob.UploadAsync(
            stream,
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType }
            },
            cancellationToken);

        return blob.Uri.ToString();
    }

    private async Task<string> SaveLocallyAsync(
        IFormFile file,
        string fileName,
        CancellationToken cancellationToken)
    {
        var uploadsPath = configuration["LocalUploads:Path"]
            ?? Path.Combine(Path.GetTempPath(), "EventManagerUploads");
        Directory.CreateDirectory(uploadsPath);
        var destination = Path.Combine(uploadsPath, fileName);

        await using var stream = File.Create(destination);
        await file.CopyToAsync(stream, cancellationToken);
        return $"/dev-uploads/{fileName}";
    }

    private static void Validate(IFormFile file)
    {
        if (file.Length == 0)
        {
            throw new InvalidOperationException("Choose a non-empty image file.");
        }

        if (file.Length > MaxFileSize)
        {
            throw new InvalidOperationException("Banner images must be 5 MB or smaller.");
        }

        var extension = Path.GetExtension(file.FileName);
        if (!AllowedExtensions.Contains(extension) || !AllowedContentTypes.Contains(file.ContentType))
        {
            throw new InvalidOperationException("Only JPG, PNG, GIF, and WebP images are accepted.");
        }
    }
}
