using Bebrakumpis.Application.Common.Result;
using Bebrakumpis.Application.Features.Auth.Commands;
using Bebrakumpis.Application.Features.Auth.Validators;
using Bebrakumpis.Application.Interfaces;
using Bebrakumpis.Domain.Entities;
using Bebrakumpis.Domain.Interfaces;
using Moq;

namespace Bebrakumpis.Tests.Application.Auth;

public class LoginCommandTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly LoginCommandHandler _handler;

    public LoginCommandTests()
    {
        _handler = new LoginCommandHandler(
            _userRepoMock.Object,
            _tokenServiceMock.Object,
            _passwordHasherMock.Object,
            new LoginCommandValidator());
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        var user = new User { Id = Guid.NewGuid(), Username = "admin", PasswordHash = "hash", Role = "Admin" };
        _userRepoMock.Setup(r => r.GetByUsernameAsync("admin", default)).ReturnsAsync(user);
        _passwordHasherMock.Setup(h => h.Verify("Admin@123", "hash")).Returns(true);
        _tokenServiceMock.Setup(t => t.GenerateToken(user)).Returns("jwt-token");

        var result = await _handler.HandleAsync(new LoginCommand("admin", "Admin@123"), default);

        Assert.True(result.IsSuccess);
        Assert.Equal("jwt-token", result.Value);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnUnauthorized_WhenUserNotFound()
    {
        _userRepoMock.Setup(r => r.GetByUsernameAsync("unknown", default)).ReturnsAsync((User?)null);
        _passwordHasherMock.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

        var result = await _handler.HandleAsync(new LoginCommand("unknown", "anything"), default);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Unauthorized, result.ErrorType);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnUnauthorized_WhenPasswordIsWrong()
    {
        var user = new User { Id = Guid.NewGuid(), Username = "admin", PasswordHash = "hash", Role = "Admin" };
        _userRepoMock.Setup(r => r.GetByUsernameAsync("admin", default)).ReturnsAsync(user);
        _passwordHasherMock.Setup(h => h.Verify("wrongpassword", "hash")).Returns(false);

        var result = await _handler.HandleAsync(new LoginCommand("admin", "wrongpassword"), default);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Unauthorized, result.ErrorType);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnValidationFailure_WhenRequestIsInvalid()
    {
        var result = await _handler.HandleAsync(new LoginCommand("", ""), default);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.ValidationFailure, result.ErrorType);
        _userRepoMock.Verify(r => r.GetByUsernameAsync(It.IsAny<string>(), default), Times.Never);
    }
}
