using RGroupConstruction.Application.Resources;
using FluentValidation;

namespace RGroupConstruction.Application.Features.Layout.Commands.CreateLayout;

internal class CreateLayoutCommandValidator : AbstractValidator<CreateLayoutCommand>
{
    public CreateLayoutCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(MessageKeys.Layout.NameRequired);
    }
}

