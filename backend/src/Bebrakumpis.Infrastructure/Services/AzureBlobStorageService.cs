using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Bebrakumpis.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Bebrakumpis.Infrastructure.Services;

public class AzureBlobStorageService(IConfiguration configuration) : IBlobStorageService
{
    private BlobContainerClient GetContainer()
    {
        var connectionString = configuration["AzureBlobStorage:ConnectionString"]!;
        var containerName = configuration["AzureBlobStorage:ContainerName"]!;
        return new BlobContainerClient(connectionString, containerName);
    }

    public async Task<string> UploadAsync(Stream content, string contentType, string blobName, CancellationToken cancellationToken = default)
    {
        var container = GetContainer();
        var blobClient = container.GetBlobClient(blobName);
        await blobClient.UploadAsync(content, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);
        return blobClient.Uri.ToString();
    }

    public async Task DeleteByUrlAsync(string blobUrl, CancellationToken cancellationToken = default)
    {
        var container = GetContainer();
        var blobName = new Uri(blobUrl).Segments.Last();
        var blobClient = container.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }
}
