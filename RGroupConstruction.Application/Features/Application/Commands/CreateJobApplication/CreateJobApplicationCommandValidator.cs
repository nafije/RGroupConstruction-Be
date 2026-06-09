using RGroupConstruction.Application.Resources;
using FluentValidation;

namespace RGroupConstruction.Application.Features.Application.Commands.CreateJobApplication;

internal class CreateJobApplicationCommandValidator : AbstractValidator<CreateJobApplicationCommand>
{
    public CreateJobApplicationCommandValidator()
    {
        RuleFor(x => x.JobId)
            .NotEmpty().WithMessage(MessageKeys.JobApplication.JobIdRequired); 

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage(MessageKeys.JobApplication.FullNameRequired);

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage(MessageKeys.JobApplication.PhoneRequired); //TODO check regex later
    }
}
