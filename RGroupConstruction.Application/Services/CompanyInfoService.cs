using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.CompanyInfo.Commands.AddCompanyInfo;
using RGroupConstruction.Application.Features.CompanyInfo.Queries.GetCompanyInfo;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Common.Constants;
using RGroupConstruction.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace CarRentalZaimi.Application.Services;

public class CompanyInfoService(
    IUnitOfWork _unitOfWork,
    ILogger<CompanyInfoService> _logger,
    IMessageLocalizer _localizer) : ICompanyInfoService
{
    public async Task<Result<CompanyInfoDto>> AddCompanyInfoDataAsync(
        AddCompanyInfoCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Saving company info profile data");

        var existingInfo = await _unitOfWork.Repository<CompanyInfo>()
            .AsQueryable()
            .Include(p => p.CompanyInfoTranslations)
            .FirstOrDefaultAsync(x => !x.IsDeleted, cancellationToken);

        bool isNew = existingInfo is null;
        _logger.Info("Company info profile will be {Operation}", isNew ? "created" : "updated");

        if (existingInfo is null)
        {
            existingInfo = new CompanyInfo
            {
                Email = request.Email,
                Phone = request.Phone,
                SalesPhoneNumber = request.SalesPhoneNumber,
                Address = request.Address,
                WorkingHours = request.WorkingHours,
                FacebookUrl = request.FacebookUrl,
                InstagramUrl = request.InstagramUrl,
                TwiterUrl = request.TwiterUrl,
                YoutubeUrl = request.YoutubeUrl,
                Years = request.Years,
                Projects = request.Projects,
                Clients = request.Clients
            };

            var defaultTranslation = request.CompanyInfoTranslations.FirstOrDefault(t =>
                t.LanguageCode!.Equals(SupportedLanguages.DefaultCode, StringComparison.OrdinalIgnoreCase));

            existingInfo.Name = defaultTranslation?.Name ?? request.CompanyInfoTranslations.FirstOrDefault()?.Name;
            existingInfo.Tagline = defaultTranslation?.Tagline;
            existingInfo.AboutText = defaultTranslation?.AboutText;
            existingInfo.MissionTitle = defaultTranslation?.MissionTitle;
            existingInfo.MissionDescription = defaultTranslation?.MissionDescription;
            existingInfo.WhyChooseUs = defaultTranslation?.WhyChooseUs;

            await _unitOfWork.Repository<CompanyInfo>().AddAsync(existingInfo, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            foreach (var t in request.CompanyInfoTranslations)
            {
                var translation = new CompanyInfoTranslation
                {
                    LanguageCode = t.LanguageCode?.ToLowerInvariant(),
                    Name = t.Name,
                    Tagline = t.Tagline,
                    AboutText = t.AboutText,
                    MissionTitle = t.MissionTitle,
                    MissionDescription = t.MissionDescription,
                    WhyChooseUs = t.WhyChooseUs,
                    CompanyInfo = existingInfo
                };
                await _unitOfWork.Repository<CompanyInfoTranslation>().AddAsync(translation, cancellationToken);
            }
        }
        else
        {

            existingInfo.Email = request.Email;
            existingInfo.Phone = request.Phone;
            existingInfo.SalesPhoneNumber = request.SalesPhoneNumber;
            existingInfo.Address = request.Address;
            existingInfo.WorkingHours = request.WorkingHours;
            existingInfo.FacebookUrl = request.FacebookUrl;
            existingInfo.InstagramUrl = request.InstagramUrl;
            existingInfo.TwiterUrl = request.TwiterUrl;
            existingInfo.YoutubeUrl = request.YoutubeUrl;
            existingInfo.Years = request.Years;
            existingInfo.Projects = request.Projects;
            existingInfo.Clients = request.Clients;

            var defaultTranslation = request.CompanyInfoTranslations.FirstOrDefault(t =>
                t.LanguageCode!.Equals(SupportedLanguages.DefaultCode, StringComparison.OrdinalIgnoreCase));

            existingInfo.Name = defaultTranslation?.Name ?? request.CompanyInfoTranslations.FirstOrDefault()?.Name ?? existingInfo.Name;
            existingInfo.Tagline = defaultTranslation?.Tagline ?? existingInfo.Tagline;
            existingInfo.AboutText = defaultTranslation?.AboutText ?? existingInfo.AboutText;
            existingInfo.MissionTitle = defaultTranslation?.MissionTitle ?? existingInfo.MissionTitle;
            existingInfo.MissionDescription = defaultTranslation?.MissionDescription ?? existingInfo.MissionDescription;
            existingInfo.WhyChooseUs = defaultTranslation?.WhyChooseUs ?? existingInfo.WhyChooseUs;

            await _unitOfWork.Repository<CompanyInfo>().UpdateAsync(existingInfo);

            foreach (var t in request.CompanyInfoTranslations)
            {
                var lang = t.LanguageCode?.ToLowerInvariant();
                var existing = existingInfo.CompanyInfoTranslations!.FirstOrDefault(x =>
                    x.LanguageCode == lang && !x.IsDeleted);

                if (existing != null)
                {
                    existing.Name = t.Name;
                    existing.Tagline = t.Tagline;
                    existing.AboutText = t.AboutText;
                    existing.MissionTitle = t.MissionTitle;
                    existing.MissionDescription = t.MissionDescription;
                    existing.WhyChooseUs = t.WhyChooseUs;
                    await _unitOfWork.Repository<CompanyInfoTranslation>().UpdateAsync(existing);
                }
                else
                {
                    var translation = new CompanyInfoTranslation
                    {
                        LanguageCode = lang,
                        Name = t.Name,
                        Tagline = t.Tagline,
                        AboutText = t.AboutText,
                        MissionTitle = t.MissionTitle,
                        MissionDescription = t.MissionDescription,
                        WhyChooseUs = t.WhyChooseUs,
                        CompanyInfo = existingInfo
                    };
                    await _unitOfWork.Repository<CompanyInfoTranslation>().AddAsync(translation, cancellationToken);
                }
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var info = await _unitOfWork.Repository<CompanyInfo>()
            .AsQueryable()
            .Include(p => p.CompanyInfoTranslations!.Where(t => !t.IsDeleted))
            .FirstOrDefaultAsync(x => !x.IsDeleted, cancellationToken);

        _logger.Info("Company info profile {Operation} successfully with id {CompanyInfoId}", isNew ? "created" : "updated", info?.Id);

        return Result<CompanyInfoDto>.Success(MapToDto(info!, SupportedLanguages.DefaultCode));
    }

    public async Task<Result<CompanyInfoDto>> GetCompanyProfileDataAsync(
        GetCompanyInfoQuery request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Getting company info profile");

        var existingInfo = await _unitOfWork.Repository<CompanyInfo>()
            .AsQueryable()
            .Include(p => p.CompanyInfoTranslations!.Where(t => !t.IsDeleted))
            .FirstOrDefaultAsync(x => !x.IsDeleted, cancellationToken);

        if (existingInfo is null)
        {
            _logger.Warn("Company info profile was not found");
            return Result<CompanyInfoDto>.Error(_localizer[MessageKeys.Error.CompanyInfo.NotFound]);
        }

        var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        _logger.Info("Retrieved company info profile {CompanyInfoId} for language {LanguageCode}", existingInfo.Id, lang);
        return Result<CompanyInfoDto>.Success(MapToDto(existingInfo, lang));
    }

    private static CompanyInfoDto MapToDto(CompanyInfo info, string lang)
    {
        var translation = info.CompanyInfoTranslations!.FirstOrDefault(t => t.LanguageCode == lang)
                       ?? info.CompanyInfoTranslations!.FirstOrDefault(t => t.LanguageCode == SupportedLanguages.DefaultCode);

        return new CompanyInfoDto
        {
            Id = info.Id,
            CreatedOn = info.CreatedOn,
            ModifiedOn = info.ModifiedOn,
            Name = translation?.Name ?? info.Name,
            Tagline = translation?.Tagline ?? info.Tagline,
            AboutText = translation?.AboutText ?? info.AboutText,
            MissionTitle = translation?.MissionTitle ?? info.MissionTitle,
            MissionDescription = translation?.MissionDescription ?? info.MissionDescription,
            WhyChooseUs = translation?.WhyChooseUs ?? info.WhyChooseUs,
            Email = info.Email,
            Phone = info.Phone,
            SalesPhoneNumber = info.SalesPhoneNumber,
            Address = info.Address,
            WorkingHours = info.WorkingHours,
            FacebookUrl = info.FacebookUrl,
            InstagramUrl = info.InstagramUrl,
            TwiterUrl = info.TwiterUrl,
            YoutubeUrl = info.YoutubeUrl,
            Years = info.Years,
            Projects = info.Projects,
            Clients = info.Clients,
            CompanyInfoTranslations = info.CompanyInfoTranslations!.Select(t => new CompanyInfoTranslationDto
            {
                LanguageCode = t.LanguageCode,
                Name = t.Name,
                Tagline = t.Tagline,
                AboutText = t.AboutText,
                MissionTitle = t.MissionTitle,
                MissionDescription = t.MissionDescription,
                WhyChooseUs = t.WhyChooseUs
            }).ToList()
        };
    }
  
}

