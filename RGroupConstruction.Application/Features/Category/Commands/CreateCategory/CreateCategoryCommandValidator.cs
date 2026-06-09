using RGroupConstruction.Application.Resources;
using FluentValidation;

namespace RGroupConstruction.Application.Features.Category.Commands.CreateCategory;

internal class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(MessageKeys.Category.NameRequired);
    }
}
