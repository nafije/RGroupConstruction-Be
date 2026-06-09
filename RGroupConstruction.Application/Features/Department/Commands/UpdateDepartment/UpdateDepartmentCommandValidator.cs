using RGroupConstruction.Application.Resources;
using FluentValidation;

namespace RGroupConstruction.Application.Features.Department.Commands.UpdateDepartment;

internal class UpdateDepartmentCommandValidator : AbstractValidator<UpdateDepartmentCommand>
{
    public UpdateDepartmentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(MessageKeys.Department.IdRequired);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(MessageKeys.Department.NameRequired);
    }
}

