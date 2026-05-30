namespace Bebrakumpis.Application.Interfaces;

public interface IBlobStorageService
{
    Task<string> UploadAsync(Stream content, string contentType, string blobName, CancellationToken cancellationToken = default);
    Task DeleteByUrlAsync(string blobUrl, CancellationToken cancellationToken = default);
}
