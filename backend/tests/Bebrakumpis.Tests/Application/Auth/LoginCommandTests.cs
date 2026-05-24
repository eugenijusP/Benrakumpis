using Bebrakumpis.Application.Features.Auth.Commands;
using Bebrakumpis.Application.Features.Auth.DTOs;
using Bebrakumpis.Application.Interfaces;
using Bebrakumpis.Domain.Entities;
using Bebrakumpis.Domain.Exceptions;
using Bebrakumpis.Domain.Interfaces;
using Moq;

namespace Bebrakumpis.Tests.Application.Auth;

public class LoginCommandTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly LoginCommand _sut;

    public LoginCommandTests()
    {
        _sut = new LoginCommand(_userRepoMock.Object, _tokenServiceMock.Object, _passwordHasherMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        var user = new User { Id = Guid.NewGuid(), Username = "admin", PasswordHash = "hash", Role = "Admin" };
        _userRepoMock.Setup(r => r.GetByUsernameAsync("admin", default)).ReturnsAsync(user);
        _passwordHasherMock.Setup(h => h.Verify("Admin@123", "hash")).Returns(true);
        _tokenServiceMock.Setup(t => t.GenerateToken(user)).Returns("jwt-token");

        var result = await _sut.ExecuteAsync(new LoginRequest { Username = "admin", Password = "Admin@123" });

        Assert.True(result.IsSuccess);
        Assert.Equal("jwt-token", result.Value);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnUnauthorized_WhenUserNotFound()
    {
        _userRepoMock.Setup(r => r.GetByUsernameAsync("unknown", default)).ReturnsAsync((User?)null);
        _passwordHasherMock.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

        var result = await _sut.ExecuteAsync(new LoginRequest { Username = "unknown", Password = "anything" });

        Assert.False(result.IsSuccess);
        Assert.Equal(Bebrakumpis.Application.Common.Result.ErrorType.Unauthorized, result.ErrorType);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnUnauthorized_WhenPasswordIsWrong()
    {
        var user = new User { Id = Guid.NewGuid(), Username = "admin", PasswordHash = "hash", Role = "Admin" };
        _userRepoMock.Setup(r => r.GetByUsernameAsync("admin", default)).ReturnsAsync(user);
        _passwordHasherMock.Setup(h => h.Verify("wrongpassword", "hash")).Returns(false);

        var result = await _sut.ExecuteAsync(new LoginRequest { Username = "admin", Password = "wrongpassword" });

        Assert.False(result.IsSuccess);
        Assert.Equal(Bebrakumpis.Application.Common.Result.ErrorType.Unauthorized, result.ErrorType);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowDomainValidationException_WhenRequestIsInvalid()
    {
        var request = new LoginRequest { Username = "", Password = "" };

        await Assert.ThrowsAsync<DomainValidationException>(
            () => _sut.ExecuteAsync(request));
    }
}
