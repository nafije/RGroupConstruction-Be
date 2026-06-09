using RGroupConstruction.Application.Resources;
using FluentValidation;

namespace RGroupConstruction.Application.Features.Layout.Commands.UpdateLayout;

internal class UpdateLayoutCommandValidator : AbstractValidator<UpdateLayoutCommand>
{
    public UpdateLayoutCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(MessageKeys.Layout.IdRequired);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(MessageKeys.Layout.NameRequired);
    }
}
