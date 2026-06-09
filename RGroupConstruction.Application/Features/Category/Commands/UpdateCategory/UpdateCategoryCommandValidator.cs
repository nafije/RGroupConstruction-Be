using RGroupConstruction.Application.Resources;
using FluentValidation;

namespace RGroupConstruction.Application.Features.Category.Commands.UpdateCategory;

internal class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(MessageKeys.Category.IdRequired);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(MessageKeys.Category.NameRequired);
    }
}
