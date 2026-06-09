using RGroupConstruction.Application.Resources;
using FluentValidation;

namespace RGroupConstruction.Application.Features.Status.Commands.UpdateStatus;

internal class UpdateStatusCommandValidator : AbstractValidator<UpdateStatusCommand>
{
    public UpdateStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(MessageKeys.Layout.IdRequired);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(MessageKeys.Status.NameRequired);
    }
}

