using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Common.Model;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Ads.Commands.CreateAds;
using RGroupConstruction.Application.Features.Ads.Commands.DeleteAds;
using RGroupConstruction.Application.Features.Ads.Commands.UpdateAds;
using RGroupConstruction.Application.Features.Ads.Queries.GetAllAds;
using RGroupConstruction.Application.Features.Ads.Queries.GetAllPagedAds;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Common.Constants;
using RGroupConstruction.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace RGroupConstruction.Application.Services;

public class AdsService(
    IUnitOfWork _unitOfWork,
    ILogger<AdsService> _logger,
    IMessageLocalizer _localizer) : IAdsService
{
    public async Task<Result<AdsDto>> CreateAsync(CreateAdsCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Creating ad with title {AdTitle}", request.Title);

        var newAds = new Ads
        {
            Title = GetDefaultTitle(request.Title, request.AdsTranslations),
            Description = GetDefaultDescription(request.Description, request.AdsTranslations), 
            LinkUrl = request.LinkUrl,
            ImageName = request.ImageName,
            VideoName = request.VideoName,
            ImageUrl = string.Empty,
            VideoUrl = string.Empty,
            IsActive = request.IsActive,
        };

        await _unitOfWork.Repository<Ads>().AddAsync(newAds, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await SyncAdsTranslationsAsync(newAds, request.Title, request.Description, request.AdsTranslations, cancellationToken);

        if (!string.IsNullOrEmpty(request.ImageData) && !string.IsNullOrEmpty(request.ImageName))
        {
            byte[] imageBytes;
            try
            {
                imageBytes = ParseBase64(request.ImageData);
            }
            catch (FormatException)
            {
                _logger.Warn("Invalid image base64 data while creating ad {AdId} with image {ImageName}", newAds.Id, request.ImageName);
                return Result<AdsDto>.Error(_localizer[MessageKeys.Error.Ads.InvalidImageBase64]);
            }

            var folderPath = Path.Combine("wwwroot", "images", "ads");
            Directory.CreateDirectory(folderPath);

            var imageName = $"{newAds.Id}_{request.ImageName}";
            var diskPath = Path.Combine(folderPath, imageName);
            var relativePath = Path.Combine("images", "ads", imageName);

            await using (var fileStream = new FileStream(diskPath, FileMode.Create))
            {
                await fileStream.WriteAsync(imageBytes, 0, imageBytes.Length, cancellationToken);
            }

            newAds.ImageUrl = relativePath;
        }

        if (!string.IsNullOrEmpty(request.VideoData) && !string.IsNullOrEmpty(request.VideoName))
        {
            byte[] videoBytes;
            try
            {
                videoBytes = ParseBase64(request.VideoData);
            }
            catch (FormatException)
            {
                _logger.Warn("Invalid video base64 data while creating ad {AdId} with video {VideoName}", newAds.Id, request.VideoName);
                return Result<AdsDto>.Error(_localizer[MessageKeys.Error.Ads.InvalidVideoBase64]);
            }

            var folderPath = Path.Combine("wwwroot", "videos", "ads");
            Directory.CreateDirectory(folderPath);

            var videoName = $"{newAds.Id}_{request.VideoName}";
            var diskPath = Path.Combine(folderPath, videoName);
            var relativePath = Path.Combine("videos", "ads", videoName);

            await using (var fileStream = new FileStream(diskPath, FileMode.Create))
            {
                await fileStream.WriteAsync(videoBytes, 0, videoBytes.Length, cancellationToken);
            }

            newAds.VideoUrl = relativePath;
        }

        await _unitOfWork.Repository<Ads>().UpdateAsync(newAds, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.Info("Created ad {AdId}", newAds.Id);

        return Result<AdsDto>.Success(MapToDto(newAds, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName));
    }

    public async Task<Result<bool>> DeleteAsync(DeleteAdsCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Deleting ad {AdId}", request.Id);

        var existingAds = await _unitOfWork.Repository<Ads>()
            .AsQueryable()
            .Include(a => a.AdsTranslations!.Where(t => !t.IsDeleted))
            .FirstOrDefaultAsync(p => p.Id.ToString() == request.Id && !p.IsDeleted, cancellationToken);

        if (existingAds is null)
        {
            _logger.Warn("Ad {AdId} was not found for delete", request.Id);
            return Result<bool>.Error(_localizer[MessageKeys.Error.Ads.NotFoundById]);
        }

        existingAds.IsDeleted = true;

        foreach (var translation in existingAds.AdsTranslations ?? [])
            translation.IsDeleted = true;

        await _unitOfWork.Repository<Ads>().UpdateAsync(existingAds, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.Info("Deleted ad {AdId}", request.Id);

        return Result<bool>.Success(true);
    }

    public async Task<Result<IEnumerable<AdsDto>>> GetAllAsync(GetAllAdsQuery request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Getting active ads");

        var adds = await _unitOfWork.Repository<Ads>()
          .AsQueryable()
          .Include(a => a.AdsTranslations!.Where(t => !t.IsDeleted))
          .Where(a => a.IsActive && !a.IsDeleted)
          .ToListAsync(cancellationToken);

        var languageCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var mapped = adds.Select(ad => MapToDto(ad, languageCode));
        _logger.Info("Retrieved {AdCount} active ads", adds.Count);
        return Result.Success(mapped);
    }

    public async Task<Result<PagedResponse<AdsDto>>> GetAllPagedAsync(GetAllPagedAdsQuery request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Getting paged ads page {PageNumber} with size {PageSize} and search {Search}", request.PageNr, request.PageSize, request.Search);

        var query = _unitOfWork.Repository<Ads>()
             .AsQueryable()
             .Include(a => a.AdsTranslations!.Where(t => !t.IsDeleted))
             .Where(c => !c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(c =>
                (c.Title != null && c.Title.ToLower().Contains(search)) ||
                c.AdsTranslations!.Any(t => t.Title != null && t.Title.ToLower().Contains(search)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var ads = await query
            .Skip((request.PageNr - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var languageCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var mapped = ads.Select(ad => MapToDto(ad, languageCode)).ToList();

        var pagedResponse = new PagedResponse<AdsDto>(mapped, totalCount, request.PageNr, request.PageSize);

        _logger.Info("Retrieved {AdCount} ads from total {TotalCount}", mapped.Count, totalCount);

        return Result<PagedResponse<AdsDto>>.Success(pagedResponse);
    }

    public async Task<Result<AdsDto>> UpdateAsync(UpdateAdsCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Updating ad {AdId}", request.Id);

        var existingAds = await _unitOfWork.Repository<Ads>()
            .AsQueryable()
            .Include(a => a.AdsTranslations!.Where(t => !t.IsDeleted))
            .FirstOrDefaultAsync(a => a.Id.ToString() == request.Id && !a.IsDeleted, cancellationToken);

        if (existingAds is null)
        {
            _logger.Warn("Ad {AdId} was not found for update", request.Id);
            return Result<AdsDto>.Error(_localizer[MessageKeys.Error.Ads.NotFound]);
        }

        existingAds.Title = GetDefaultTitle(request.Title, request.AdsTranslations) ?? existingAds.Title; 
        existingAds.Description = GetDefaultDescription(request.Description, request.AdsTranslations) ?? existingAds.Description;
        existingAds.LinkUrl = request.LinkUrl;
        existingAds.IsActive = request.IsActive;

        await SyncAdsTranslationsAsync(existingAds, request.Title, request.Description, request.AdsTranslations, cancellationToken);

        bool hasNewImage = !string.IsNullOrEmpty(request.ImageData) && !string.IsNullOrEmpty(request.ImageName);
        bool hasNewVideo = !string.IsNullOrEmpty(request.VideoData) && !string.IsNullOrEmpty(request.VideoName);

        if (hasNewImage && !string.IsNullOrEmpty(existingAds.VideoUrl))
        {
            DeleteFromDisk(existingAds.VideoUrl);
            existingAds.VideoUrl = string.Empty;
            existingAds.VideoName = string.Empty;
        }

        if (hasNewVideo && !string.IsNullOrEmpty(existingAds.ImageUrl))
        {
            DeleteFromDisk(existingAds.ImageUrl);
            existingAds.ImageUrl = string.Empty;
            existingAds.ImageName = string.Empty;
        }

        if (hasNewImage)
        {
            DeleteFromDisk(existingAds.ImageUrl);

            byte[] imageBytes;
            try
            {
                imageBytes = ParseBase64(request.ImageData!);
            }
            catch (FormatException)
            {
                _logger.Warn("Invalid image base64 data while updating ad {AdId} with image {ImageName}", request.Id, request.ImageName);
                return Result<AdsDto>.Error(_localizer[MessageKeys.Error.Ads.InvalidImageBase64]);
            }

            var folderPath = Path.Combine("wwwroot", "images", "ads");
            Directory.CreateDirectory(folderPath);

            var imageName = $"{existingAds.Id}_{request.ImageName}";
            var diskPath = Path.Combine(folderPath, imageName);
            var relativePath = Path.Combine("images", "ads", imageName);

            await File.WriteAllBytesAsync(diskPath, imageBytes, cancellationToken);

            existingAds.ImageUrl = relativePath;
            existingAds.ImageName = request.ImageName;
        }

        if (hasNewVideo)
        {
            DeleteFromDisk(existingAds.VideoUrl);

            byte[] videoBytes;
            try
            {
                videoBytes = ParseBase64(request.VideoData!);
            }
            catch (FormatException)
            {
                _logger.Warn("Invalid video base64 data while updating ad {AdId} with video {VideoName}", request.Id, request.VideoName);
                return Result<AdsDto>.Error(_localizer[MessageKeys.Error.Ads.InvalidVideoBase64]);
            }

            var folderPath = Path.Combine("wwwroot", "videos", "ads");
            Directory.CreateDirectory(folderPath);

            var videoName = $"{existingAds.Id}_{request.VideoName}";
            var diskPath = Path.Combine(folderPath, videoName);
            var relativePath = Path.Combine("videos", "ads", videoName);

            await File.WriteAllBytesAsync(diskPath, videoBytes, cancellationToken);

            existingAds.VideoUrl = relativePath;
            existingAds.VideoName = request.VideoName;
        }

        await _unitOfWork.Repository<Ads>().UpdateAsync(existingAds, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.Info("Updated ad {AdId}", request.Id);

        return Result<AdsDto>.Success(MapToDto(existingAds, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName));
    }

    private async Task SyncAdsTranslationsAsync(
        Ads ads,
        string? fallbackTitle,
        string? fallbackDescription,
        IEnumerable<AdsTranslationCommand>? requestedTranslations,
        CancellationToken cancellationToken)
    {
        var translationsByLanguage = BuildTranslationValues(fallbackTitle, fallbackDescription, requestedTranslations);
        if (translationsByLanguage.Count == 0)
            return;

        ads.AdsTranslations ??= [];

        foreach (var (languageCode, values) in translationsByLanguage)
        {
            var (title, description) = values;

            var existingTranslation = ads.AdsTranslations.FirstOrDefault(t =>
                t.LanguageCode == languageCode && !t.IsDeleted);

            if (existingTranslation is not null)
            {
                existingTranslation.Title = title;
                existingTranslation.Description = description;
                await _unitOfWork.Repository<AdsTranslation>().UpdateAsync(existingTranslation, cancellationToken);
                continue;
            }

            var newTranslation = new AdsTranslation
            {
                Ads = ads,
                LanguageCode = languageCode,
                Title = title,
                Description = description
            };

            ads.AdsTranslations.Add(newTranslation);
            await _unitOfWork.Repository<AdsTranslation>().AddAsync(newTranslation, cancellationToken);
        }
    }

    private static Dictionary<string, (string Title, string Description)> BuildTranslationValues(
    string? fallbackTitle,
    string? fallbackDescription,                 
    IEnumerable<AdsTranslationCommand>? requestedTranslations)
    {
        var values = new Dictionary<string, (string, string)>(StringComparer.OrdinalIgnoreCase);

        foreach (var translation in requestedTranslations ?? [])
        {
            var languageCode = translation.LanguageCode?.ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(languageCode) ||
                !SupportedLanguages.IsSupported(languageCode) ||
                (string.IsNullOrWhiteSpace(translation.Title) && string.IsNullOrWhiteSpace(translation.Description)))
                continue;

            values[languageCode] = (translation.Title ?? string.Empty, translation.Description ?? string.Empty); 
        }

        var defaultTitle = GetDefaultTitle(fallbackTitle, requestedTranslations);
        var defaultDescription = GetDefaultDescription(fallbackDescription, requestedTranslations);

        foreach (var languageCode in SupportedLanguages.Codes)
            values.TryAdd(languageCode, (defaultTitle ?? string.Empty, defaultDescription ?? string.Empty)); 

        return values;
    }

    private static string? GetDefaultTitle(
        string? fallbackTitle,
        IEnumerable<AdsTranslationCommand>? requestedTranslations)
    {
        if (!string.IsNullOrWhiteSpace(fallbackTitle))
            return fallbackTitle;

        var defaultTranslation = requestedTranslations?.FirstOrDefault(t =>
            t.LanguageCode?.Equals(SupportedLanguages.DefaultCode, StringComparison.OrdinalIgnoreCase) == true);

        return defaultTranslation?.Title
            ?? requestedTranslations?.FirstOrDefault(t => !string.IsNullOrWhiteSpace(t.Title))?.Title;
    }

    private static string? GetDefaultDescription(
    string? fallbackDescription,
    IEnumerable<AdsTranslationCommand>? requestedTranslations)
    {
        if (!string.IsNullOrWhiteSpace(fallbackDescription))
            return fallbackDescription;

        var defaultTranslation = requestedTranslations?.FirstOrDefault(t =>
            t.LanguageCode?.Equals(SupportedLanguages.DefaultCode, StringComparison.OrdinalIgnoreCase) == true);

        return defaultTranslation?.Description
            ?? requestedTranslations?.FirstOrDefault(t => !string.IsNullOrWhiteSpace(t.Description))?.Description;
    }
    private static AdsDto MapToDto(Ads ads, string languageCode)
    {
        var translations = ads.AdsTranslations?
            .Where(t => !t.IsDeleted)
            .ToList() ?? [];

        var translation = translations.FirstOrDefault(t => t.LanguageCode == languageCode)
                       ?? translations.FirstOrDefault(t => t.LanguageCode == SupportedLanguages.DefaultCode);

        return new AdsDto
        {
            Id = ads.Id,
            CreatedOn = ads.CreatedOn,
            ModifiedOn = ads.ModifiedOn,
            Title = translation?.Title ?? ads.Title,
            Description = translation?.Description ?? ads.Description,
            AdsTranslations = translations.Select(t => new AdsTranslationDto
            {
                LanguageCode = t.LanguageCode,
                Title = t.Title,
                Description = t.Description
            }).ToList(),
            ImageName = ads.ImageName,
            ImageUrl = ads.ImageUrl,
            VideoName = ads.VideoName,
            VideoUrl = ads.VideoUrl,
            LinkUrl = ads.LinkUrl,
            IsActive = ads.IsActive
        };
    }

    private static byte[] ParseBase64(string base64Input)
    {
        var base64Data = base64Input.Contains(',')
            ? base64Input.Split(',')[1]
            : base64Input;

        return Convert.FromBase64String(base64Data);
    }

    private static void DeleteFromDisk(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return;

        var fullPath = Path.Combine("wwwroot", relativePath);

        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }
}


