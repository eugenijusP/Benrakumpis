using Bebrakumpis.Application.Features.Auth.Commands;
using FluentValidation;

namespace Bebrakumpis.Application.Features.Auth.Validators;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Password).NotEmpty().MaximumLength(200);
    }
}
