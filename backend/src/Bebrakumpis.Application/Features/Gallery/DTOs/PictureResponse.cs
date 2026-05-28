namespace Bebrakumpis.Application.Features.Gallery.DTOs;

public class PictureResponse
{
    public Guid Id { get; set; }
    public string BlobUrl { get; set; } = string.Empty;
    public int Order { get; set; }
    public DateTime UploadedAt { get; set; }
}
