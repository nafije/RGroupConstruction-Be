using RGroupConstruction.Application.Resources;
using FluentValidation;

namespace RGroupConstruction.Application.Features.ContactMessage.Commands.CreateContactMessage;

internal class CreateContactMessageCommandValidator : AbstractValidator<CreateContactMessageCommand>
{
    public CreateContactMessageCommandValidator()
    {
        RuleFor(x => x.FullName)
           .NotEmpty().WithMessage(MessageKeys.Validation.FullNameRequired)
           .MaximumLength(200).WithMessage(MessageKeys.Validation.NameMaxLength50);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(MessageKeys.Validation.EmailRequired)
            .EmailAddress().WithMessage(MessageKeys.Validation.EmailInvalid);

        RuleFor(x => x.Message)
           .NotEmpty().WithMessage(MessageKeys.Validation.AddressRequired);
    }
}

