using Bebrakumpis.Application.Common.Result;
using Bebrakumpis.Application.Features.Gallery.Commands;
using Bebrakumpis.Application.Interfaces;
using Bebrakumpis.Domain.Entities;
using Bebrakumpis.Domain.Interfaces;
using Moq;

namespace Bebrakumpis.Tests.Application.Gallery;

public class DeletePictureCommandTests
{
    private readonly Mock<IPictureRepository> _repoMock = new();
    private readonly Mock<IBlobStorageService> _blobMock = new();
    private readonly DeletePictureCommandHandler _handler;

    public DeletePictureCommandTests()
    {
        _handler = new DeletePictureCommandHandler(_repoMock.Object, _blobMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteBlobAndRecord_WhenPictureExists()
    {
        var id = Guid.NewGuid();
        var blobUrl = "https://fake.blob/photo.jpg";
        _repoMock.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(
            new Picture { Id = id, BlobUrl = blobUrl, Order = 1, UploadedAt = DateTime.UtcNow });

        var result = await _handler.HandleAsync(new DeletePictureCommand(id), default);

        Assert.True(result.IsSuccess);
        _blobMock.Verify(b => b.DeleteByUrlAsync(blobUrl, default), Times.Once);
        _repoMock.Verify(r => r.DeleteAsync(id, default), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenPictureDoesNotExist()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Picture?)null);

        var result = await _handler.HandleAsync(new DeletePictureCommand(Guid.NewGuid()), default);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
        _blobMock.Verify(b => b.DeleteByUrlAsync(It.IsAny<string>(), default), Times.Never);
        _repoMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), default), Times.Never);
    }
}
