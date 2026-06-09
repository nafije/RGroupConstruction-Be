using RGroupConstruction.Application.Resources;
using FluentValidation;

namespace RGroupConstruction.Application.Features.Subscribe.Commands.CreateSubscription;

internal class CreateSubscriptionCommandValidator : AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(MessageKeys.Validation.EmailRequired)
            .EmailAddress().WithMessage(MessageKeys.Validation.EmailInvalid);
    }
}
