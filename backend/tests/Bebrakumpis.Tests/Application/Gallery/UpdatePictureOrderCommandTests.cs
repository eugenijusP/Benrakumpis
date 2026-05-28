using Bebrakumpis.Application.Common.Result;
using Bebrakumpis.Application.Features.Gallery.Commands;
using Bebrakumpis.Application.Features.Gallery.Validators;
using Bebrakumpis.Domain.Entities;
using Bebrakumpis.Domain.Interfaces;
using Moq;

namespace Bebrakumpis.Tests.Application.Gallery;

public class UpdatePictureOrderCommandTests
{
    private readonly Mock<IPictureRepository> _repoMock = new();
    private readonly UpdatePictureOrderCommandHandler _handler;

    public UpdatePictureOrderCommandTests()
    {
        _handler = new UpdatePictureOrderCommandHandler(_repoMock.Object, new UpdatePictureOrderCommandValidator());
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnUpdatedResponse_WhenPictureExists()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(
            new Picture { Id = id, BlobUrl = "https://fake/1.jpg", Order = 1, UploadedAt = DateTime.UtcNow });
        _repoMock.Setup(r => r.UpdateOrderAsync(id, 3, default)).Returns(Task.CompletedTask);

        var result = await _handler.HandleAsync(new UpdatePictureOrderCommand(id, 3), default);

        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value.Order);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenPictureDoesNotExist()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Picture?)null);

        var result = await _handler.HandleAsync(new UpdatePictureOrderCommand(Guid.NewGuid(), 1), default);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnValidationFailure_WhenOrderIsNegative()
    {
        var result = await _handler.HandleAsync(new UpdatePictureOrderCommand(Guid.NewGuid(), -1), default);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.ValidationFailure, result.ErrorType);
        _repoMock.Verify(r => r.UpdateOrderAsync(It.IsAny<Guid>(), It.IsAny<int>(), default), Times.Never);
    }
}
