using RGroupConstruction.Application.Features.Job.Commands.CreateJob;
using RGroupConstruction.Application.Resources;
using FluentValidation;

namespace RGroupConstruction.Application.Features.Job.Commands.CreateJob;

internal class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
{
    public CreateJobCommandValidator()
    {
        {
            RuleFor(x => x.Title)
            .NotEmpty().WithMessage(MessageKeys.Job.TiteRequired);

            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage(MessageKeys.Job.DepartmentIdRequired);

            RuleFor(x => x.EmploymentType)
                .NotEmpty().WithMessage(MessageKeys.Job.EmploymentTypeRequired);
        }
    }
}
