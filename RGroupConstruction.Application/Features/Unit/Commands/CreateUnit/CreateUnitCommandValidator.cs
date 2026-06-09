using RGroupConstruction.Application.Resources;
using FluentValidation;

namespace RGroupConstruction.Application.Features.Unit.Commands.CreateUnit;

internal class CreateUnitCommandValidator : AbstractValidator<CreateUnitCommand>
{
    public CreateUnitCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage(MessageKeys.Unit.ProjectIdRequired);

        RuleFor(x => x.UnitCategoryId)
            .NotEmpty().WithMessage(MessageKeys.Unit.CategoryIdRequired);

        RuleFor(x => x.UnitStatus)
            .NotEmpty().WithMessage(MessageKeys.Unit.StatusRequired);

        RuleFor(x => x.UnitRefNumber)
            .NotEmpty().WithMessage(MessageKeys.Unit.RefNumberRequired);
    }
}

