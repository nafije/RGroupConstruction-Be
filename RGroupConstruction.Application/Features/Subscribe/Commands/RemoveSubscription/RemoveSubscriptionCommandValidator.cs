using RGroupConstruction.Application.Resources;
using FluentValidation;

namespace RGroupConstruction.Application.Features.Subscribe.Commands.RemoveSubscription;

internal class RemoveSubscriptionCommandValidator : AbstractValidator<RemoveSubscriptionCommand>
{
    public RemoveSubscriptionCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage(MessageKeys.Validation.TokenRequired);
    }
}

