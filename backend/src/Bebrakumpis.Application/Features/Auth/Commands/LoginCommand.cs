using Bebrakumpis.Application.Common.Result;
using Bebrakumpis.Application.Features.Auth.DTOs;
using Bebrakumpis.Application.Features.Auth.Validators;
using Bebrakumpis.Application.Interfaces;
using Bebrakumpis.Domain.Exceptions;
using Bebrakumpis.Domain.Interfaces;

namespace Bebrakumpis.Application.Features.Auth.Commands;

public class LoginCommand(IUserRepository userRepository, ITokenService tokenService, IPasswordHasher passwordHasher)
{
    public async Task<Result<string>> ExecuteAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var validator = new LoginRequestValidator();
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            var errors = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            throw new DomainValidationException([errors]);
        }

        var user = await userRepository.GetByUsernameAsync(request.Username, cancellationToken);
        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result<string>.Unauthorized("Invalid username or password.");

        var token = tokenService.GenerateToken(user);
        return Result<string>.Success(token);
    }
}
