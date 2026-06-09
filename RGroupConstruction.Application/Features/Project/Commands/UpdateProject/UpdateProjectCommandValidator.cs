using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Common.Constants;
using FluentValidation;

namespace RGroupConstruction.Application.Features.Project.Commands.UpdateProject;

internal class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        {
            RuleFor(x => x.ProjectTranslations)
           .NotEmpty().WithMessage(MessageKeys.Validation.AtLeastOneTranslationRequired);

            RuleForEach(x => x.ProjectTranslations).ChildRules(t =>
            {
                t.RuleFor(x => x.LanguageCode)
                    .NotEmpty().WithMessage(MessageKeys.Validation.LanguageCodeRequired)
                    .Must(SupportedLanguages.IsSupported)
                    .WithMessage(MessageKeys.Validation.LanguageCodeSupported);

                t.RuleFor(x => x.Name)
                    .NotEmpty().WithMessage(MessageKeys.Validation.TranslationNameRequired)
                    .MaximumLength(200).WithMessage(MessageKeys.Validation.NameMaxLength200);
            });

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(MessageKeys.Project.IdRequired);

            RuleFor(x => x.Name)
            .NotEmpty().WithMessage(MessageKeys.Project.NameRequired);

            RuleFor(x => x.CityId)
                .NotEmpty().WithMessage(MessageKeys.Project.CityRequired);

            RuleFor(x => x.ProjectType)
                .NotEmpty().WithMessage(MessageKeys.Project.ProjectTypeRequired);

            RuleFor(x => x.ProjectStatusId)
                .NotEmpty().WithMessage(MessageKeys.Project.ProjectStatusRequired);

            RuleFor(x => x.TotalUnits)
                .NotEmpty().WithMessage(MessageKeys.Project.TotalUnitsGraterThan0);
        }
    }
}
