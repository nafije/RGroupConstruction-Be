using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Common.Constants;
using FluentValidation;

namespace RGroupConstruction.Application.Features.Ads.Commands.UpdateAds;

internal class UpdateAdsCommandValidator : AbstractValidator<UpdateAdsCommand>
{
    public UpdateAdsCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(MessageKeys.Validation.IdRequired);

        RuleFor(x => x.Title)
           .NotEmpty().WithMessage(MessageKeys.Validation.TitleRequired)
           .When(x => x.AdsTranslations is null || x.AdsTranslations.Count == 0);

        RuleFor(x => x.Title)
           .MaximumLength(200).WithMessage(MessageKeys.Validation.TitleMaxLength200);

        RuleForEach(x => x.AdsTranslations).ChildRules(t =>
        {
            t.RuleFor(x => x.LanguageCode)
                .NotEmpty().WithMessage(MessageKeys.Validation.LanguageCodeRequired)
                .Must(SupportedLanguages.IsSupported)
                .WithMessage(MessageKeys.Validation.LanguageCodeSupported);

            t.RuleFor(x => x.Title)
                .NotEmpty().WithMessage(MessageKeys.Validation.TitleRequired)
                .MaximumLength(200).WithMessage(MessageKeys.Validation.TitleMaxLength200);
        });

    }
}

