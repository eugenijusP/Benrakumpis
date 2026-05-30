using Bebrakumpis.Application.Features.Gallery.Queries;
using Bebrakumpis.Domain.Entities;
using Bebrakumpis.Domain.Interfaces;
using Moq;

namespace Bebrakumpis.Tests.Application.Gallery;

public class GetAllPicturesQueryTests
{
    private readonly Mock<IPictureRepository> _repoMock = new();
    private readonly GetAllPicturesQueryHandler _handler;

    public GetAllPicturesQueryTests()
    {
        _handler = new GetAllPicturesQueryHandler(_repoMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmptyList_WhenNoPicturesExist()
    {
        _repoMock.Setup(r => r.GetAllAsync(default)).ReturnsAsync([]);

        var result = await _handler.HandleAsync(new GetAllPicturesQuery(), default);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPictureList_WhenPicturesExist()
    {
        var pictures = new List<Picture>
        {
            new() { Id = Guid.NewGuid(), BlobUrl = "https://fake/1.jpg", Order = 1, UploadedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), BlobUrl = "https://fake/2.jpg", Order = 2, UploadedAt = DateTime.UtcNow }
        };
        _repoMock.Setup(r => r.GetAllAsync(default)).ReturnsAsync(pictures);

        var result = await _handler.HandleAsync(new GetAllPicturesQuery(), default);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count());
    }
}
