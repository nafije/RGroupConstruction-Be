using RGroupConstruction.Application.Resources;
using FluentValidation;

namespace RGroupConstruction.Application.Features.Unit.Commands.UpdateUnit;

internal class UpdateUnitCommandValidator : AbstractValidator<UpdateUnitCommand>
{
    public UpdateUnitCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(MessageKeys.Unit.IdRequired);

        RuleFor(x => x.UnitCategoryId)
            .NotEmpty().WithMessage(MessageKeys.Unit.CategoryIdRequired);

        RuleFor(x => x.UnitStatus)
            .NotEmpty().WithMessage(MessageKeys.Unit.StatusRequired);

        RuleFor(x => x.UnitRefNumber)
            .NotEmpty().WithMessage(MessageKeys.Unit.RefNumberRequired);
    }
}

