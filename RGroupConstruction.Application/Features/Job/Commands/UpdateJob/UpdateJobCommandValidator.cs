using RGroupConstruction.Application.Resources;
using FluentValidation;

namespace RGroupConstruction.Application.Features.Job.Commands.UpdateJob;

internal class UpdateJobCommandValidator : AbstractValidator<UpdateJobCommand>
{
    public UpdateJobCommandValidator()
    {
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(MessageKeys.Job.IdRequired);

            RuleFor(x => x.Title)
            .NotEmpty().WithMessage(MessageKeys.Job.TiteRequired);

            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage(MessageKeys.Job.DepartmentIdRequired);

            RuleFor(x => x.EmploymentType)
                .NotEmpty().WithMessage(MessageKeys.Job.EmploymentTypeRequired);
        }
    }
}
