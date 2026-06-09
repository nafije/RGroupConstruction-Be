using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Common.Constants;
using FluentValidation;

namespace RGroupConstruction.Application.Features.CompanyInfo.Commands.AddCompanyInfo;

internal class AddCompanyInfoCommandValidator : AbstractValidator<AddCompanyInfoCommand>
{
    public AddCompanyInfoCommandValidator()
    {
        RuleFor(x => x.CompanyInfoTranslations)
           .NotEmpty().WithMessage(MessageKeys.Validation.AtLeastOneTranslationRequired);

        RuleForEach(x => x.CompanyInfoTranslations).ChildRules(t =>
        {
            t.RuleFor(x => x.LanguageCode)
                .NotEmpty().WithMessage(MessageKeys.Validation.LanguageCodeRequired)
                .Must(SupportedLanguages.IsSupported)
                .WithMessage(MessageKeys.Validation.LanguageCodeSupported);

            t.RuleFor(x => x.Name)
                .NotEmpty().WithMessage(MessageKeys.Validation.TranslationNameRequired)
                .MaximumLength(200).WithMessage(MessageKeys.Validation.NameMaxLength200);
        });

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(MessageKeys.Validation.EmailRequired)
            .EmailAddress().WithMessage(MessageKeys.Validation.EmailInvalid);

        RuleFor(x => x.Phone)
           .NotEmpty()
           .WithMessage(MessageKeys.Validation.PhoneNumberRequired)
           .Matches(@"^\+?[1-9]\d{1,14}$")
           .WithMessage(MessageKeys.Validation.PhoneNumberInvalidInternational);

        RuleFor(x => x.Address)
           .NotEmpty().WithMessage(MessageKeys.Validation.AddressRequired);
    }
}


