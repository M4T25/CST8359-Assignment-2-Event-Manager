namespace EventManager.Services;

public interface IBannerStorageService
{
    Task<string> SaveAsync(IFormFile file, CancellationToken cancellationToken = default);
}
