using RGroupConstruction.Application.Resources;
using FluentValidation;

namespace RGroupConstruction.Application.Features.Authentication.Command.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage(MessageKeys.Validation.LoginRequired);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(MessageKeys.Validation.PasswordRequired)
            .MinimumLength(6).WithMessage(MessageKeys.Validation.PasswordMinLength6);
    }
}
