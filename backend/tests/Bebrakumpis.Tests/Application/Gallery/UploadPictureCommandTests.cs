using Bebrakumpis.Application.Common.Result;
using Bebrakumpis.Application.Features.Gallery.Commands;
using Bebrakumpis.Application.Features.Gallery.Validators;
using Bebrakumpis.Application.Interfaces;
using Bebrakumpis.Domain.Entities;
using Bebrakumpis.Domain.Interfaces;
using Moq;

namespace Bebrakumpis.Tests.Application.Gallery;

public class UploadPictureCommandTests
{
    private readonly Mock<IPictureRepository> _repoMock = new();
    private readonly Mock<IBlobStorageService> _blobMock = new();
    private readonly UploadPictureCommandHandler _handler;

    public UploadPictureCommandTests()
    {
        _handler = new UploadPictureCommandHandler(_repoMock.Object, _blobMock.Object, new UploadPictureCommandValidator());
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPictureResponse_WhenJpegUploaded()
    {
        _repoMock.Setup(r => r.GetMaxOrderAsync(default)).ReturnsAsync(0);
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<Picture>(), default)).ReturnsAsync(Guid.NewGuid());
        _blobMock.Setup(b => b.UploadAsync(It.IsAny<Stream>(), "image/jpeg", It.IsAny<string>(), default))
            .ReturnsAsync("https://fake.blob/photo.jpg");

        using var stream = new MemoryStream(new byte[100]);
        var result = await _handler.HandleAsync(
            new UploadPictureCommand(stream, 100, "image/jpeg", "photo.jpg"), default);

        Assert.True(result.IsSuccess);
        Assert.Equal("https://fake.blob/photo.jpg", result.Value.BlobUrl);
        Assert.Equal(1, result.Value.Order);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnValidationFailure_WhenContentTypeIsInvalid()
    {
        using var stream = new MemoryStream(new byte[100]);
        var result = await _handler.HandleAsync(
            new UploadPictureCommand(stream, 100, "application/pdf", "doc.pdf"), default);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.ValidationFailure, result.ErrorType);
        _blobMock.Verify(b => b.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), default), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnValidationFailure_WhenFileSizeIsZero()
    {
        using var stream = new MemoryStream();
        var result = await _handler.HandleAsync(
            new UploadPictureCommand(stream, 0, "image/jpeg", "empty.jpg"), default);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.ValidationFailure, result.ErrorType);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnValidationFailure_WhenFileSizeExceedsLimit()
    {
        using var stream = new MemoryStream(new byte[100]);
        var result = await _handler.HandleAsync(
            new UploadPictureCommand(stream, 11 * 1024 * 1024, "image/jpeg", "big.jpg"), default);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.ValidationFailure, result.ErrorType);
    }

    [Fact]
    public async Task HandleAsync_ShouldAssignOrderAfterCurrentMax()
    {
        _repoMock.Setup(r => r.GetMaxOrderAsync(default)).ReturnsAsync(5);
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<Picture>(), default)).ReturnsAsync(Guid.NewGuid());
        _blobMock.Setup(b => b.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), default))
            .ReturnsAsync("https://fake.blob/photo.jpg");

        using var stream = new MemoryStream(new byte[100]);
        var result = await _handler.HandleAsync(
            new UploadPictureCommand(stream, 100, "image/png", "photo.png"), default);

        Assert.True(result.IsSuccess);
        Assert.Equal(6, result.Value.Order);
    }
}
