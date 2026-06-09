using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Common.Model;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Unit.Commands.CreateUnit;
using RGroupConstruction.Application.Features.Unit.Commands.DeleteUnit;
using RGroupConstruction.Application.Features.Unit.Commands.UpdateUnit;
using RGroupConstruction.Application.Features.Unit.Queries.GetAllPagedUnits;
using RGroupConstruction.Application.Features.Unit.Queries.GetAllUnits;
using RGroupConstruction.Application.Features.Unit.Queries.GetSimilarlUnits;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Entities;
using RGroupConstruction.Domain.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace RGroupConstruction.Application.Services;

public class UnitService(IUnitOfWork _uow, IMapper _mapper, ILogger<UnitService> _logger, IMessageLocalizer _localizer) : IUnitService
{
  
    public async Task<Result<UnitDto>> CreateUnitAsync(CreateUnitCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Creating unit {UnitRefNumber} for project {ProjectId}", request.UnitRefNumber, request.ProjectId);

        if (request.UnitRefNumber is not null)
        {
            var existingProject = await _uow.Repository<Unit>()
                .FirstOrDefaultAsync(c => c.UnitRefNumber == request.UnitRefNumber, cancellationToken);

            if (existingProject is not null)
            {
                _logger.Warn("Unit {UnitRefNumber} already exists", request.UnitRefNumber);
                return Result<UnitDto>.Error(_localizer[MessageKeys.Error.Unit.UnitExists]);
            }
        }

        var project = await _uow.Repository<Project>()
               .FirstOrDefaultAsync(p => p.Id.ToString() == request.ProjectId && !p.IsDeleted, cancellationToken);

        if (project is null)
        {
            _logger.Warn("Project {ProjectId} was not found while creating unit {UnitRefNumber}", request.ProjectId, request.UnitRefNumber);
            return Result<UnitDto>.Error(_localizer[MessageKeys.Error.Unit.ProjectNotFound]);
        }

        Layout? layout = null;
        if (!string.IsNullOrEmpty(request.LayoutId))
        {
            layout = await _uow.Repository<Layout>()
                  .FirstOrDefaultAsync(p => p.Id.ToString() == request.LayoutId && !p.IsDeleted, cancellationToken);

            if (layout is null)
            {
                _logger.Warn("Layout {LayoutId} was not found while creating unit {UnitRefNumber}", request.LayoutId, request.UnitRefNumber);
                return Result<UnitDto>.Error(_localizer[MessageKeys.Error.Unit.LayoutNotFound]);
            }
        }

        var category = await _uow.Repository<Category>()
              .FirstOrDefaultAsync(p => p.Id.ToString() == request.UnitCategoryId && !p.IsDeleted, cancellationToken);

        if (category is null)
        {
            _logger.Warn("Category {CategoryId} was not found while creating unit {UnitRefNumber}", request.UnitCategoryId, request.UnitRefNumber);
            return Result<UnitDto>.Error(_localizer[MessageKeys.Error.Unit.CategoryNotFound]);
        }


        Enum.TryParse<UnitStatus>(request.UnitStatus, out var status);

        await _uow.BeginTransactionAsync(cancellationToken);

        try
        {
            var newUnit = new Unit
            {
                Project = project,
                Layout = layout,
                UnitCategory = category,
                UnitStatus = status,
                UnitRefNumber = request.UnitRefNumber,
                Floor = request.Floor,
                BathRooms = request.BathRooms,
                BedRooms = request.BedRooms,
                NetArea = request.NetArea,
                GrossArea = request.GrossArea,
                PriceM2 = request.PriceM2,
                TotalPrice = request.TotalPrice,
                TerraceAre = request.TerraceAre,
            };

            await _uow.Repository<Unit>().AddAsync(newUnit, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            await SaveUnitFloorPlanAsync(newUnit, request.FlorPlanFileUrl, request.FlorPlanFileName, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            // Add images if provided
            if (request.UnitImages is not null && request.UnitImages.Count > 0)
                await AddUnitImagesAsync(newUnit, request.UnitImages, cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);
            await _uow.CommitTransactionAsync(cancellationToken);

            _logger.Info("Created unit {UnitId} with reference {UnitRefNumber}", newUnit.Id, newUnit.UnitRefNumber);

            return Result<UnitDto>.Success(_mapper.Map<UnitDto>(newUnit));
        }
        catch (Exception ex)
        {
            await _uow.RollbackTransactionAsync(cancellationToken);
            _logger.Error(ex, "Failed to create unit {UnitRefNumber}", request.UnitRefNumber);
            return Result<UnitDto>.Error(_localizer[MessageKeys.Error.Unit.CreateFailed]);
        }
    }

    public async Task<Result<bool>> DeleteUnitAsync(DeleteUnitCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Deleting unit {UnitId}", request.Id);

        var existingUnit = await _uow.Repository<Unit>()
          .FirstOrDefaultAsync(c => c.Id.ToString() == request.Id && !c.IsDeleted, cancellationToken);

        if (existingUnit is null)
        {
            _logger.Warn("Unit {UnitId} was not found for delete", request.Id);
            return Result<bool>.Error(_localizer[MessageKeys.Error.Unit.NotFound]);
        }

        // Soft-delete all associated images and remove files from disk
        var unitImages = await _uow.Repository<UnitImage>()
            .AsQueryable()
            .Where(i => i.Unit!.Id == existingUnit.Id && !i.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var image in unitImages)
        {
            image.IsDeleted = true;

            if (!string.IsNullOrEmpty(image.ImagePath))
            {
                var filePath = Path.Combine("wwwroot", image.ImagePath);
                if (File.Exists(filePath))
                {
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                    _logger.Info("Deleted unit image file {ImagePath} for unit {UnitId}", image.ImagePath, request.Id);
                }
            }
        }

        existingUnit.IsDeleted = true; 
        await SaveUnitFloorPlanAsync(existingUnit, null, null, cancellationToken);


        await _uow.Repository<Unit>().UpdateAsync(existingUnit, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        _logger.Info("Deleted unit {UnitId}", request.Id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<PagedResponse<UnitDto>>> GetAllPagedUnitsAsync(GetAllPagedUnitsQuery request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Getting paged units page {PageNumber} with size {PageSize}, search {Search}, project {ProjectId}, category {CategoryId}, layout {LayoutId}", request.PageNr, request.PageSize, request.Search, request.ProjectId, request.CategoryId, request.LayoutId);

        var query = _uow.Repository<Unit>()
            .AsQueryable()
            .Include(u => u.Project)
            .Include(u => u.UnitCategory)
            .Include(u => u.Layout)
            .Include(u => u.UnitImages!.Where(i => !i.IsDeleted))
            .Where(c => !c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();

            query = query.Where(c =>
                (c.UnitRefNumber != null && c.UnitRefNumber.ToLower().Contains(search)) ||
                (c.UnitCategory != null && c.UnitCategory.Name != null && c.UnitCategory.Name.ToLower().Contains(search)) ||
                (c.Project != null && c.Project.Name != null && c.Project.Name.ToLower().Contains(search)) ||
                (c.Layout != null && c.Layout.Name != null && c.Layout.Name.ToLower().Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(request.ProjectId))
            query = query.Where(c => c.Project != null && c.Project.Id.ToString() == request.ProjectId);

        if (!string.IsNullOrWhiteSpace(request.CategoryId))
            query = query.Where(c => c.UnitCategory != null && c.UnitCategory.Id.ToString() == request.CategoryId);

        if (!string.IsNullOrWhiteSpace(request.LayoutId))
            query = query.Where(c => c.Layout != null && c.Layout.Id.ToString() == request.LayoutId);

        if (request.MinPrixe > 0)
            query = query.Where(c => c.TotalPrice >= request.MinPrixe);

        if (request.MaxPrice > 0)
            query = query.Where(c => c.TotalPrice <= request.MaxPrice);

        if (request.MinGrosArea > 0)
            query = query.Where(c => c.GrossArea >= request.MinGrosArea);

        if (request.MaxGrosArea > 0)
            query = query.Where(c => c.GrossArea <= request.MaxGrosArea);

        var totalCount = await query.CountAsync(cancellationToken);

        var units = await query
            .OrderByDescending(c => c.CreatedOn)
            .Skip((request.PageNr - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var mapped = _mapper.Map<List<UnitDto>>(units);

        var pagedResponse = new PagedResponse<UnitDto>(mapped, totalCount, request.PageNr, request.PageSize);

        _logger.Info("Retrieved {UnitCount} units from total {TotalCount}", mapped.Count, totalCount);

        return Result<PagedResponse<UnitDto>>.Success(pagedResponse);
    }

    public async Task<Result<IEnumerable<UnitDto>>> GetAllUnitsAsync(GetAllUnitsQuery request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Getting all units");

        var units = await _uow.Repository<Unit>()
        .AsQueryable()
        .Where(u => !u.IsDeleted &&
                !_uow.Repository<UnitClient>()
                     .AsQueryable()
                     .Where(uc => !uc.IsDeleted)
                     .Select(uc => uc.Unit!.Id)
                     .Contains(u.Id))
       .ToListAsync(cancellationToken);

        var mapped = _mapper.Map<List<UnitDto>>(units);


        _logger.Info("Retrieved {ProjectCount} units", mapped.Count);
        return Result.Success<IEnumerable<UnitDto>>(mapped);
    }

    public async Task<Result<IEnumerable<UnitDto>>> GetSimilarUnitsAsync(GetSimilarlUnitsQuery request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Getting similar units for unit {UnitId}", request.UnitId);

        var unit = await _uow.Repository<Unit>()
            .AsQueryable()
            .Include(u => u.UnitCategory)
            .Include(u => u.Layout)
            .FirstOrDefaultAsync(u => u.Id.ToString() == request.UnitId && !u.IsDeleted, cancellationToken);

        if (unit is null)
        {
            _logger.Warn("Unit {UnitId} was not found for similar units query", request.UnitId);
            return Result<IEnumerable<UnitDto>>.Error(_localizer[MessageKeys.Error.Unit.NotFound]);
        }

        // Pre-filter: same category is mandatory, exclude self
        var candidates = await _uow.Repository<Unit>()
            .AsQueryable()
            .Include(u => u.Project)
            .Include(u => u.UnitCategory)
            .Include(u => u.Layout)
            .Include(u => u.UnitImages!.Where(i => !i.IsDeleted))
            .Where(u =>
                !u.IsDeleted &&
                u.Id != unit.Id &&
                u.UnitCategory != null &&
                u.UnitCategory.Id == unit.UnitCategory!.Id)
            .ToListAsync(cancellationToken);

        var result = candidates
            .Select(c => new { Unit = c, Score = ComputeSimilarityScore(unit, c) })
            .OrderByDescending(x => x.Score)
            .Take(4)
            .Select(x => x.Unit)
            .ToList();

        _logger.Info("Retrieved {Count} similar units for unit {UnitId}", result.Count, request.UnitId);
        return Result<IEnumerable<UnitDto>>.Success(_mapper.Map<List<UnitDto>>(result));
    }


    public async Task<Result<UnitDto>> UpdateUnitAsync(UpdateUnitCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Updating unit {UnitId}", request.Id);

        var unit = await _uow.Repository<Unit>()
            .FirstOrDefaultAsync(u => u.Id.ToString() == request.Id && !u.IsDeleted, cancellationToken);

        if (unit is null)
        {
            _logger.Warn("Unit {UnitId} was not found for update", request.Id);
            return Result<UnitDto>.Error(_localizer[MessageKeys.Error.Unit.NotFound]);
        }

        
        // Layout is optional — only fetch and validate if LayoutId is provided
        Layout? layout = null;
        if (!string.IsNullOrEmpty(request.LayoutId))
        {
            layout = await _uow.Repository<Layout>()
                .FirstOrDefaultAsync(p => p.Id.ToString() == request.LayoutId && !p.IsDeleted, cancellationToken);

            if (layout is null)
            {
                _logger.Warn("Layout {LayoutId} was not found while updating unit {UnitId}", request.LayoutId, request.Id);
                return Result<UnitDto>.Error(_localizer[MessageKeys.Error.Unit.LayoutNotFound]);
            }
        }

        var category = await _uow.Repository<Category>()
            .FirstOrDefaultAsync(p => p.Id.ToString() == request.UnitCategoryId && !p.IsDeleted, cancellationToken);

        if (category is null)
        {
            _logger.Warn("Category {CategoryId} was not found while updating unit {UnitId}", request.UnitCategoryId, request.Id);
            return Result<UnitDto>.Error(_localizer[MessageKeys.Error.Unit.CategoryNotFound]);
        }

        Enum.TryParse<UnitStatus>(request.UnitStatus, out var status);

        await _uow.BeginTransactionAsync(cancellationToken);

        try
        {
            unit.Layout = layout;    
            unit.UnitCategory = category;
            unit.UnitStatus = status;
            unit.UnitRefNumber = request.UnitRefNumber;
            unit.Floor = request.Floor;
            unit.BathRooms = request.BathRooms;
            unit.BedRooms = request.BedRooms;
            unit.NetArea = request.NetArea;
            unit.GrossArea = request.GrossArea;
            unit.PriceM2 = request.PriceM2;
            unit.TotalPrice = request.TotalPrice;
            unit.TerraceAre = request.TerraceAre;

            await SaveUnitFloorPlanAsync(unit, request.FlorPlanFileUrl, request.FlorPlanFileName, cancellationToken);

            await _uow.Repository<Unit>().UpdateAsync(unit, cancellationToken);

            if (request.UnitImages is not null && request.UnitImages.Count > 0)
                await UpdateUnitImagesAsync(unit, request.UnitImages, cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);
            await _uow.CommitTransactionAsync(cancellationToken);

            _logger.Info("Updated unit {UnitId}", request.Id);
            return Result<UnitDto>.Success(_mapper.Map<UnitDto>(unit));
        }
        catch (Exception ex)
        {
            await _uow.RollbackTransactionAsync(cancellationToken);
            _logger.Error(ex, "Failed to update unit {UnitId}", request.Id);
            return Result<UnitDto>.Error(_localizer[MessageKeys.Error.Unit.UpdateFailed]);
        }

    }
    private async Task AddUnitImagesAsync(Unit unit, List<UnitImagesCommand> images, CancellationToken cancellationToken)
    {
        var folderName = $"{unit.Id}";
        var folderPath = Path.Combine("wwwroot", "images", "units", folderName);
        Directory.CreateDirectory(folderPath);

        var primaryImageName = images.FirstOrDefault(i => i.IsPrimary)?.Name;

        var unitImages = new List<UnitImage>();

        foreach (var image in images)
        {
            if (string.IsNullOrEmpty(image.Name))
                continue;

            byte[] imageData;
            try
            {
                imageData = Convert.FromBase64String(image.Data!);
            }
            catch (FormatException)
            {
                _logger.Warn("Invalid base64 data for unit image {ImageName} on unit {UnitId}", image.Name, unit.Id);
                continue;
            }

            var diskPath = Path.Combine(folderPath, image.Name);
            var relativePath = Path.Combine("images", "units", folderName, image.Name);

            await using (var fileStream = new FileStream(diskPath, FileMode.Create))
            {
                await fileStream.WriteAsync(imageData, 0, imageData.Length);
            }

            unitImages.Add(new UnitImage
            {
                Unit = unit,
                ImageName = image.Name,
                ImagePath = relativePath,
                IsPrimary = image.Name == primaryImageName
            });
        }

        // Fallback: mark first image as primary if none was flagged
        if (unitImages.Count > 0 && !unitImages.Any(i => i.IsPrimary))
            unitImages[0].IsPrimary = true;

        await _uow.Repository<UnitImage>().AddRangeAsync(unitImages, cancellationToken);
        _logger.Info("Added {ImageCount} image(s) to unit {UnitId}", unitImages.Count, unit.Id);
    }


    private async Task UpdateUnitImagesAsync(Unit unit, List<UnitImagesCommand> images, CancellationToken cancellationToken)
    {
        var folderName = $"{unit.Id}";
        var folderPath = Path.Combine("wwwroot", "images", "units", folderName);
        Directory.CreateDirectory(folderPath);

        // Get all existing non-deleted images for this car
        var existingImages = await _uow.Repository<UnitImage>()
            .AsQueryable()
            .Where(i => i.Unit!.Id == unit.Id && !i.IsDeleted)
            .ToListAsync(cancellationToken);

        // Soft-delete images that are no longer in the incoming list
        var incomingImageNames = images
            .Where(i => !string.IsNullOrEmpty(i.Name))
            .Select(i => i.Name)
            .ToHashSet();

        foreach (var imageToDelete in existingImages.Where(x => !incomingImageNames.Contains(x.ImageName)))
        {
            imageToDelete.IsDeleted = true;

            if (!string.IsNullOrEmpty(imageToDelete.ImagePath))
            {
                var oldFilePath = Path.Combine("wwwroot", imageToDelete.ImagePath);
                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                    _logger.Info("Deleted old unit image file {ImagePath} for unit {UnitId}", imageToDelete.ImagePath, unit.Id);
                }
            }
        }

        // Determine new primary
        var primaryImageName = images.FirstOrDefault(i => i.IsPrimary)?.Name;

        foreach (var image in images)
        {
            if (string.IsNullOrEmpty(image.Name))
                continue;

            bool isPrimary = image.Name == primaryImageName;

            var existingImage = existingImages.FirstOrDefault(x => x.ImageName == image.Name);

            // No base64 data means image already exists — only update IsPrimary
            if (string.IsNullOrEmpty(image.Data))
            {
                if (existingImage is null)
                {
                    _logger.Warn("Unit image {ImageName} was not found in database for unit {UnitId}", image.Name, unit.Id);
                    continue;
                }

                existingImage.IsPrimary = isPrimary;
                await _uow.Repository<UnitImage>().UpdateAsync(existingImage, cancellationToken);
                continue;
            }

            byte[] imageData;
            try
            {
                imageData = Convert.FromBase64String(image.Data);
            }
            catch (FormatException)
            {
                _logger.Warn("Invalid base64 data for unit image {ImageName} on unit {UnitId}", image.Name, unit.Id);
                continue;
            }

            var diskPath = Path.Combine(folderPath, image.Name);
            var relativePath = Path.Combine("images", "units", folderName, image.Name);

            if (existingImage is not null)
            {
                // Delete old file from disk
                if (!string.IsNullOrEmpty(existingImage.ImagePath))
                {
                    var oldFilePath = Path.Combine("wwwroot", existingImage.ImagePath);
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                        _logger.Info("Deleted replaced unit image file {ImagePath} for unit {UnitId}", existingImage.ImagePath, unit.Id);
                    }
                }

                await using (var fileStream = new FileStream(diskPath, FileMode.Create))
                {
                    await fileStream.WriteAsync(imageData, 0, imageData.Length);
                }

                existingImage.ImageName = image.Name;
                existingImage.ImagePath = relativePath;
                existingImage.IsPrimary = isPrimary;
                existingImage.IsDeleted = false;

                await _uow.Repository<UnitImage>().UpdateAsync(existingImage, cancellationToken);
            }
            else
            {
                await using (var fileStream = new FileStream(diskPath, FileMode.Create))
                {
                    await fileStream.WriteAsync(imageData, 0, imageData.Length);
                }

                var newImage = new UnitImage
                {
                    Unit = unit,
                    ImageName = image.Name,
                    ImagePath = relativePath,
                    IsPrimary = isPrimary
                };

                await _uow.Repository<UnitImage>().AddAsync(newImage, cancellationToken);
            }
        }

        _logger.Info("Updated unit images for unit {UnitId}", unit.Id);
    }

    private static int ComputeSimilarityScore(Unit source, Unit candidate)
    {
        int score = 0;

        // Same layout
        if (source.Layout?.Id != null && source.Layout.Id == candidate.Layout?.Id)
            score += 4;

        // Same bedroom count
        if (source.BedRooms == candidate.BedRooms)
            score += 3;

        // Same bathroom count
        if (source.BathRooms == candidate.BathRooms)
            score += 2;

        // Total price within ±15%
        if (source.TotalPrice > 0 && candidate.TotalPrice > 0)
        {
            var priceDiff = Math.Abs(source.TotalPrice - candidate.TotalPrice) / source.TotalPrice;
            if (priceDiff <= 0.15m) score += 2;
            else if (priceDiff <= 0.30m) score += 1;
        }

        return score;
    }

    private async Task SaveUnitFloorPlanAsync(Unit unit, string? base64Data, string? fileName, CancellationToken cancellationToken)
    {
        // Delete old file if exists
        if (!string.IsNullOrEmpty(unit.FlorPlanFileUrl))
        {
            var oldFilePath = Path.Combine("wwwroot", unit.FlorPlanFileUrl);
            if (File.Exists(oldFilePath))
            {
                File.Delete(oldFilePath);
                _logger.Info("Deleted old unit floor plan file {FilePath} for unit {UnitId}", unit.FlorPlanFileUrl, unit.Id);
            }

            unit.FlorPlanFileUrl = null;
            unit.FlorPlanFileName = null;
        }

        if (string.IsNullOrEmpty(base64Data) || string.IsNullOrEmpty(fileName))
            return;

        // Strip the data URL prefix if present (e.g. "data:application/pdf;base64,")
        var pureBase64 = base64Data.Contains(',')
            ? base64Data[(base64Data.IndexOf(',') + 1)..]
            : base64Data;

        byte[] pdfData;
        try
        {
            pdfData = Convert.FromBase64String(pureBase64);
        }
        catch (FormatException)
        {
            _logger.Warn("Invalid base64 PDF data for floor plan on unit {UnitId}", unit.Id);
            return;
        }

        var folderPath = Path.Combine("wwwroot", "images", "units", $"{unit.Id}", "plans");
        Directory.CreateDirectory(folderPath);

        var relativePath = Path.Combine("images", "units", $"{unit.Id}", "plans", fileName);
        var diskPath = Path.Combine("wwwroot", relativePath);

        await using var fs = new FileStream(diskPath, FileMode.Create);
        await fs.WriteAsync(pdfData, 0, pdfData.Length, cancellationToken);

        unit.FlorPlanFileName = fileName;
        unit.FlorPlanFileUrl  = relativePath;

        _logger.Info("Saved unit floor plan {FileName} for unit {UnitId}", fileName, unit.Id);
    }

}

