using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Common.Model;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Project.Commands.AddFeaturedProject;
using RGroupConstruction.Application.Features.Project.Commands.CreateProject;
using RGroupConstruction.Application.Features.Project.Commands.DeleteProject;
using RGroupConstruction.Application.Features.Project.Commands.UpdateProject;
using RGroupConstruction.Application.Features.Project.Queries.GetAllPagedProjects;
using RGroupConstruction.Application.Features.Project.Queries.GetAllProjects;
using RGroupConstruction.Application.Features.Project.Queries.GetFeaturedProjects;
using RGroupConstruction.Application.Features.Project.Queries.GetProjectById;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Common.Constants;
using RGroupConstruction.Domain.Entities;
using RGroupConstruction.Domain.Enums;
using RGroupConstruction.Logging;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace RGroupConstruction.Application.Services;

public class ProjectService(IUnitOfWork _uow, IMapper _mapper, ILogger<ProjectService> _logger, IMessageLocalizer _localizer, IEmailService _emailService) : IProjectService
{
    public async Task<Result<ProjectDto>> CreateProjectAsync(CreateProjectCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Creating project {ProjectName}", request.Name);

        if (request.Name is not null)
        {
            var existingProject = await _uow.Repository<Project>()
                .FirstOrDefaultAsync(c => c.Name == request.Name, cancellationToken);

            if (existingProject is not null)
            {
                _logger.Warn("Project {ProjectName} already exists", request.Name);
                return Result<ProjectDto>.Error(_localizer[MessageKeys.Error.Project.ProjectExists]);
            }
        }

        Enum.TryParse<ProjectType>(request.ProjectType, out var type);

        var status = await _uow.Repository<Status>()
             .FirstOrDefaultAsync(p => p.Id.ToString() == request.ProjectStatusId && !p.IsDeleted, cancellationToken);

        if (status is null)
        {
            _logger.Warn("Status {StatusId} was not found while creating project {ProjectName}", request.ProjectStatusId, request.Name);
            return Result<ProjectDto>.Error(_localizer[MessageKeys.Error.Project.StatusNotFound]);
        }

        var city = await _uow.Repository<City>()
            .FirstOrDefaultAsync(p => p.Id.ToString() == request.CityId && !p.IsDeleted, cancellationToken);

        if (status is null)
        {
            _logger.Warn("City {CityId} was not found while creating project {ProjectName}", request.CityId, request.Name);
            return Result<ProjectDto>.Error(_localizer[MessageKeys.Error.Project.StatusNotFound]);
        }

        await _uow.BeginTransactionAsync(cancellationToken);

        try
        {
            var defaultTranslation = request.ProjectTranslations?.FirstOrDefault(t =>
                t.LanguageCode!.Equals(SupportedLanguages.DefaultCode, StringComparison.OrdinalIgnoreCase));


            var newProject = new Project
            {
                Name = request.Name,
                Year = request.Year,
                City = city,
                Description = request.Description,
                ProjectType = type,
                ProjectStatus = status,
                Location = request.Location,
                Facilities = request.Facilities,
                TotalUnits = request.TotalUnits,
                ResidentialUnits = request.ResidentialUnits,
                ComercialUnits = request.ComercialUnits,
                ParkingUnits = request.ParkingUnits,

            };

            await _uow.Repository<Project>().AddAsync(newProject, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            await SaveProjectFloorPlanAsync(newProject, request.ProjectPlanFileUrl, request.ProjectPlanFileName, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            // Add translations if provided
            if (request.ProjectTranslations is not null && request.ProjectTranslations.Count > 0)
                foreach (var t in request.ProjectTranslations)
                {
                    var translation = new ProjectTranslation
                    {
                        LanguageCode = t.LanguageCode?.ToLowerInvariant(),
                        Name = t.Name,
                        Description = t.Description,
                        Facilities = t.Facilities,
                        Project = newProject
                    };
                    await _uow.Repository<ProjectTranslation>().AddAsync(translation, cancellationToken);
                }

            // Add images if provided
            if (request.ProjectImages is not null && request.ProjectImages.Count > 0)
                await AddProjectImagesAsync(newProject, request.ProjectImages, cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);
            await _uow.CommitTransactionAsync(cancellationToken);
            _logger.Info("Created project {ProjectId}", newProject.Id);

            //send email of new project to subscribers
            var subscriberEmails = await _uow.Repository<Subscribe>()
                .AsQueryable()
                .Where(s => !s.IsDeleted)
                .Select(s => s.Email!)
                .ToListAsync(cancellationToken);


            var primaryImage = newProject.ProjectImages?.FirstOrDefault(img => img.IsPrimary);

            _logger.Info("Sending new-project emails for project {ProjectId} to {SubscriberCount} subscriber(s)", newProject.Id, subscriberEmails.Count);

            foreach (var email in subscriberEmails)
            {
                var result = await _emailService.SendNewProjectEmailToUserAsync(
                    projectName: newProject.Name ?? string.Empty,
                    projectType: newProject.ProjectType.ToString(),
                    city: newProject.City!.Name ?? string.Empty,
                    description: newProject.Description ?? string.Empty,
                    userEmail: email,
                    projectId: newProject.Id,
                    imageUrl: primaryImage?.ImagePath ?? string.Empty,
                    cancellationToken: cancellationToken);

                if (!result.IsSuccessful)
                    _logger.Warn("Failed to send new-project email to {SubscriberEmail} for project {ProjectId}", email, newProject.Id);
            }

            var dto = _mapper.Map<ProjectDto>(newProject);
            var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            ApplyTranslations(dto, lang);

            return Result<ProjectDto>.Success(dto);
        }
        catch (Exception ex)
        {
            await _uow.RollbackTransactionAsync(cancellationToken);
            _logger.Error(ex, "Failed to create project {ProjectName}", request.Name);
            return Result<ProjectDto>.Error(_localizer[MessageKeys.Error.Project.CreateFailed]);
        }

    }

    public async Task<Result<bool>> DeleteProjectAsync(DeleteProjectCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Deleting project {ProjectId}", request.Id);

        var existingProject = await _uow.Repository<Project>()
           .FirstOrDefaultAsync(c => c.Id.ToString() == request.Id && !c.IsDeleted, cancellationToken);

        if (existingProject is null)
        {
            _logger.Warn("Project {ProjectId} was not found for delete", request.Id);
            return Result<bool>.Error(_localizer[MessageKeys.Error.Project.NotFound]);
        }

        // Soft-delete all associated images and remove files from disk
        var projectImages = await _uow.Repository<ProjectImage>()
            .AsQueryable()
            .Where(i => i.Project!.Id == existingProject.Id && !i.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var image in projectImages)
        {
            image.IsDeleted = true;

            if (!string.IsNullOrEmpty(image.ImagePath))
            {
                if (!string.IsNullOrEmpty(image.ImagePath))
                {
                    var filePath = Path.Combine("wwwroot", image.ImagePath);
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
            }
        }

        existingProject.IsDeleted = true;
        await SaveProjectFloorPlanAsync(existingProject, null, null, cancellationToken);

        await _uow.Repository<Project>().UpdateAsync(existingProject, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        _logger.Info("Deleted project {ProjectId}", request.Id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<PagedResponse<ProjectDto>>> GetAllPagedProjectsAsync(
        GetAllPagedProjectsQuery request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Getting paged projects page {PageNumber} with size {PageSize} and search {Search}", request.PageNr, request.PageSize, request.Search);

        var query = _uow.Repository<Project>()
            .AsQueryable()
            .Include(p => p.ProjectTranslations!.Where(t => !t.IsDeleted))
            .Include(p => p.ProjectImages!.Where(i => !i.IsDeleted))
            .Where(c => !c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(c =>
                (c.Name != null && c.Name.ToLower().Contains(request.Search.ToLower())) || 
                (c.Description != null && c.Description.ToLower().Contains(request.Search.ToLower())));

        if (!string.IsNullOrWhiteSpace(request.ProjectType) &&
            Enum.TryParse<ProjectType>(request.ProjectType, true, out var projectType))
        {
            query = query.Where(p => p.ProjectType == projectType);
        }

        if (!string.IsNullOrWhiteSpace(request.ProjectStatusId))
        {
            query = query.Where(p => p.ProjectStatus!.Id.ToString() == request.ProjectStatusId);
        }

        if (!string.IsNullOrWhiteSpace(request.CityId))
        {
            query = query.Where(p => p.City!.Id.ToString() == request.CityId);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var projects = await query
            .Skip((request.PageNr - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<ProjectDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        ApplyTranslations(projects, lang);

        var projectIds = projects.Select(p => p.Id).ToList();

        var unitStats = await _uow.Repository<Unit>()
            .AsQueryable()
            .Where(u => projectIds.Contains(u.Project!.Id) && !u.IsDeleted
                     && (u.UnitStatus == UnitStatus.Reserved || u.UnitStatus == UnitStatus.Sold))
            .GroupBy(u => new
            {
                u.Project!.Id,
                u.Project.ResidentialUnits,
                u.Project.ComercialUnits
            })
            .Select(g => new
            {
                ProjectId = g.Key.Id,
                BusyResidentialUnits = g.Count(u => u.UnitCategory!.Name == "Apartment"),
                BusyCommercialUnits = g.Count(u => u.UnitCategory!.Name == "Shop"),
            })
            .ToDictionaryAsync(x => x.ProjectId, cancellationToken);

        var parkingStats = await _uow.Repository<Parking>()
            .AsQueryable()
            .Where(p => projectIds.Contains(p.Project!.Id) && !p.IsDeleted)
            .GroupBy(p => p.Project!.Id)
            .Select(g => new
            {
                ProjectId = g.Key,
                AvailableParkingUnits = g.Sum(p => p.AvailableParking),
            })
            .ToDictionaryAsync(x => x.ProjectId, cancellationToken);

        foreach (var project in projects)
        {
            if (unitStats.TryGetValue(project.Id, out var units))
            {
                project.AvailableResidentialUnits = project.ResidentialUnits - units.BusyResidentialUnits;
                project.AvailableCcomercialUnits  = project.ComercialUnits - units.BusyCommercialUnits;
            }

            if (parkingStats.TryGetValue(project.Id, out var parking))
            {
                project.AvailableParkingUnits = parking.AvailableParkingUnits;
            }
        }

        _logger.Info("Retrieved {ProjectCount} projects from total {TotalCount}", projects.Count, totalCount);

        return Result<PagedResponse<ProjectDto>>.Success(
            new PagedResponse<ProjectDto>(projects, totalCount, request.PageNr, request.PageSize));
    }

    public async Task<Result<ProjectDto>> GetProjectByIdAsync(
    GetProjectByIdQuery request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Getting project by id {ProjectId}", request.ProjectId);

        var project = await _uow.Repository<Project>()
            .AsQueryable()
            .Include(p => p.ProjectTranslations!.Where(t => !t.IsDeleted))
            .Include(p => p.ProjectImages!.Where(i => !i.IsDeleted))
            .Where(p => p.Id.ToString() == request.ProjectId && !p.IsDeleted)
            .ProjectTo<ProjectDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (project is null)
        {
            _logger.Warn("Project with id {ProjectId} not found", request.ProjectId);
            return Result<ProjectDto>.Error("Project not found");
        }

        var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        ApplyTranslations(new List<ProjectDto> { project }, lang);

        var unitStats = await _uow.Repository<Unit>()
            .AsQueryable()
            .Where(u => u.Project!.Id == project.Id && !u.IsDeleted
                     && (u.UnitStatus == UnitStatus.Reserved || u.UnitStatus == UnitStatus.Sold))
            .GroupBy(u => new
            {
                u.Project!.Id,
                u.Project.ResidentialUnits,
                u.Project.ComercialUnits
            })
            .Select(g => new
            {
                ProjectId = g.Key.Id,
                BusyResidentialUnits = g.Count(u => u.UnitCategory!.Name == "Apartment"),
                BusyCommercialUnits = g.Count(u => u.UnitCategory!.Name == "Shop"),
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (unitStats is not null)
        {
            project.AvailableResidentialUnits = project.ResidentialUnits - unitStats.BusyResidentialUnits;
            project.AvailableCcomercialUnits  = project.ComercialUnits - unitStats.BusyCommercialUnits;
        }

        var parkingStats = await _uow.Repository<Parking>()
            .AsQueryable()
            .Where(p => p.Project!.Id == project.Id && !p.IsDeleted)
            .Select(p => new { AvailableParkingUnits = p.AvailableParking })
            .FirstOrDefaultAsync(cancellationToken);

        if (parkingStats is not null)
        {
            project.AvailableParkingUnits = parkingStats.AvailableParkingUnits;
        }

        _logger.Info("Retrieved project {ProjectId}", project.Id);
        return Result<ProjectDto>.Success(project);
    }

    public async Task<Result<IEnumerable<ProjectDto>>> GetAllProjectsAsync(GetAllProjectsQuery request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Getting all projects");

        var projects = await _uow.Repository<Project>()
            .AsQueryable()
            .Include(p => p.ProjectTranslations!.Where(t => !t.IsDeleted))
            .Where(c => !c.IsDeleted)
            .ToListAsync(cancellationToken);

        var mapped = _mapper.Map<List<ProjectDto>>(projects);

        var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        ApplyTranslations(mapped, lang);

        _logger.Info("Retrieved {ProjectCount} projects", mapped.Count);
        return Result.Success<IEnumerable<ProjectDto>>(mapped);
    }

    public async Task<Result<IEnumerable<ProjectDto>>> GetFeaturedProjectsAsync(
       GetFeaturedProjectsQuery request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Getting featured projects with limit {Limit}", request.Limit);

        var projects = await _uow.Repository<Project>()
            .AsQueryable()
            .Include(p => p.ProjectTranslations!.Where(t => !t.IsDeleted))
            .Include(p => p.ProjectImages!.Where(i => !i.IsDeleted))
            .Where(c => c.IsFeatured && !c.IsDeleted)
            .ProjectTo<ProjectDto>(_mapper.ConfigurationProvider)
            .Take(request.Limit)
            .ToListAsync(cancellationToken);

        var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        ApplyTranslations(projects, lang);

        if (projects.Count == 0)
        {
            _logger.Info("No featured projects found");
            return Result<IEnumerable<ProjectDto>>.Success(projects);
        }

        var projectIds = projects.Select(p => p.Id).ToList();

        var unitStats = await _uow.Repository<Unit>()
            .AsQueryable()
            .Where(u => projectIds.Contains(u.Project!.Id) && !u.IsDeleted
                     && (u.UnitStatus == UnitStatus.Reserved || u.UnitStatus == UnitStatus.Sold))
            .GroupBy(u => new
            {
                u.Project!.Id,
                u.Project.ResidentialUnits,
                u.Project.ComercialUnits
            })
            .Select(g => new
            {
                ProjectId = g.Key.Id,
                BusyResidentialUnits = g.Count(u => u.UnitCategory!.Name == "Apartment"),
                BusyCommercialUnits = g.Count(u => u.UnitCategory!.Name == "Shop"),
            })
            .ToDictionaryAsync(x => x.ProjectId, cancellationToken);

        var parkingStats = await _uow.Repository<Parking>()
            .AsQueryable()
            .Where(p => projectIds.Contains(p.Project!.Id) && !p.IsDeleted)
            .GroupBy(p => p.Project!.Id)
            .Select(g => new
            {
                ProjectId = g.Key,
                AvailableParkingUnits = g.Sum(p => p.AvailableParking),
            })
            .ToDictionaryAsync(x => x.ProjectId, cancellationToken);

        foreach (var project in projects)
        {
            if (unitStats.TryGetValue(project.Id, out var units))
            {
                project.AvailableResidentialUnits = project.ResidentialUnits - units.BusyResidentialUnits;
                project.AvailableCcomercialUnits  = project.ComercialUnits - units.BusyCommercialUnits;
            }

            if (parkingStats.TryGetValue(project.Id, out var parking))
            {
                project.AvailableParkingUnits = parking.AvailableParkingUnits;
            }
        }

        _logger.Info("Retrieved {ProjectCount} featured projects", projects.Count);
        return Result<IEnumerable<ProjectDto>>.Success(projects);
    }

    public async Task<Result<ProjectDto>> UpdateProjectAsync(UpdateProjectCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Updating project {ProjectId}", request.Id);

        var existingProject = await _uow.Repository<Project>()
            .AsQueryable()
            .Include(p => p.ProjectStatus)
            .Include(p => p.ProjectTranslations!.Where(t => !t.IsDeleted))
            .FirstOrDefaultAsync(c => c.Id.ToString() == request.Id, cancellationToken);

        if (existingProject is null)
        {
            _logger.Warn("Project {ProjectId} was not found for update", request.Id);
            return Result<ProjectDto>.Error(_localizer[MessageKeys.Error.Project.NotFound]);
        }

        if (request.Name is not null)
        {
            var duplicate = await _uow.Repository<Project>()
                .FirstOrDefaultAsync(c => c.Name == request.Name && c.Id.ToString() != request.Id, cancellationToken);

            if (duplicate is not null)
            {
                _logger.Warn("Project name {ProjectName} already exists for another project", request.Name);
                return Result<ProjectDto>.Error(_localizer[MessageKeys.Error.Project.ProjectExists]);
            }
        }

        Enum.TryParse<ProjectType>(request.ProjectType, out var type);

        var status = await _uow.Repository<Status>()
            .FirstOrDefaultAsync(p => p.Id.ToString() == request.ProjectStatusId && !p.IsDeleted, cancellationToken);

        if (status is null)
        {
            _logger.Warn("Status {StatusId} was not found while updating project {ProjectId}", request.ProjectStatusId, request.Id);
            return Result<ProjectDto>.Error(_localizer[MessageKeys.Error.Project.StatusNotFound]);
        }

        var city = await _uow.Repository<City>()
            .FirstOrDefaultAsync(p => p.Id.ToString() == request.CityId && !p.IsDeleted, cancellationToken);

        if (status is null)
        {
            _logger.Warn("City {CityId} was not found while creating project {ProjectName}", request.CityId, request.Name);
            return Result<ProjectDto>.Error(_localizer[MessageKeys.Error.Project.StatusNotFound]);
        }

        await _uow.BeginTransactionAsync(cancellationToken);

        var oldStatus = existingProject.ProjectStatus;

        try
        {
            // Update main fields from default translation if present
            var defaultTranslation = request.ProjectTranslations?.FirstOrDefault(t =>
                t.LanguageCode!.Equals(SupportedLanguages.DefaultCode, StringComparison.OrdinalIgnoreCase));

            existingProject.Name = defaultTranslation?.Name
                ?? request.ProjectTranslations?.FirstOrDefault()?.Name
                ?? existingProject.Name;

            existingProject.Description = defaultTranslation?.Description
                ?? request.ProjectTranslations?.FirstOrDefault()?.Description
                ?? existingProject.Description;

            existingProject.Facilities = defaultTranslation?.Facilities
                ?? request.ProjectTranslations?.FirstOrDefault()?.Facilities
                ?? existingProject.Facilities;

            existingProject.City = city;
            existingProject.Year = request.Year;
            existingProject.ProjectType = type;
            existingProject.ProjectStatus = status;
            existingProject.Location = request.Location;
            existingProject.TotalUnits = request.TotalUnits;
            existingProject.ComercialUnits = request.ComercialUnits;
            existingProject.ResidentialUnits = request.ResidentialUnits;
            existingProject.ParkingUnits = request.ParkingUnits;

            await SaveProjectFloorPlanAsync(existingProject, request.ProjectPlanFileUrl, request.ProjectPlanFileName, cancellationToken);

            // ✅ Upsert translations
            if (request.ProjectTranslations is not null && request.ProjectTranslations.Count > 0)
                foreach (var t in request.ProjectTranslations)
                {
                    var translationLang = t.LanguageCode?.ToLowerInvariant();

                    var existingTranslation = existingProject.ProjectTranslations!
                        .FirstOrDefault(x => x.LanguageCode == translationLang && !x.IsDeleted);

                    if (existingTranslation is not null)
                    {
                        existingTranslation.Name = t.Name;
                        existingTranslation.Description = t.Description;
                        existingTranslation.Facilities = t.Facilities;

                        await _uow.Repository<ProjectTranslation>()
                            .UpdateAsync(existingTranslation, cancellationToken);
                    }
                    else
                    {
                        var newTranslation = new ProjectTranslation
                        {
                            LanguageCode = translationLang,
                            Name = t.Name,
                            Description = t.Description,
                            Facilities = t.Facilities,
                            Project = existingProject
                        };

                        await _uow.Repository<ProjectTranslation>()
                            .AddAsync(newTranslation, cancellationToken);
                    }
                }

            // ✅ Images
            if (request.ProjectImages is not null && request.ProjectImages.Count > 0)
                await UpdateProjectImagesAsync(existingProject, request.ProjectImages, cancellationToken);

            await _uow.Repository<Project>().UpdateAsync(existingProject, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            await _uow.CommitTransactionAsync(cancellationToken);
            _logger.Info("Updated project {ProjectId}", request.Id);

            //send email to subscribers for status change
            if (oldStatus != status)
            {
                var subscriberEmails = await _uow.Repository<Subscribe>()
                    .AsQueryable()
                    .Where(s => !s.IsDeleted)
                    .Select(s => s.Email!)
                    .ToListAsync(cancellationToken);

                var primaryImage = existingProject.ProjectImages?.FirstOrDefault(img => img.IsPrimary);

                _logger.Info("Sending project status update emails for project {ProjectId} to {SubscriberCount} subscriber(s)", existingProject.Id, subscriberEmails.Count);

                foreach (var email in subscriberEmails)
                {
                    var result = await _emailService.SendProjectUpdateEmailToUserAsync(
                        projectName: existingProject.Name ?? string.Empty,
                        oldStatus: oldStatus!.Name!.ToString()!,
                        newStatus: status!.Name!.ToString()!,
                        city: city!.Name ?? string.Empty,
                        userEmail: email,
                        projectId: existingProject.Id,
                        imageUrl: primaryImage?.ImagePath ?? string.Empty,
                        cancellationToken: cancellationToken);

                    if (!result.IsSuccessful)
                        _logger.Warn("Failed to send status-update email to {SubscriberEmail} for project {ProjectId}", email, existingProject.Id);
                }
            }
            // ✅ Reload → Map → ApplyTranslations
            var updated = await _uow.Repository<Project>()
                .AsQueryable()
                .Include(p => p.ProjectTranslations!.Where(t => !t.IsDeleted))
                .FirstOrDefaultAsync(p => p.Id == existingProject.Id, cancellationToken);

            var dto = _mapper.Map<ProjectDto>(updated);

            var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            ApplyTranslations(dto, lang);

            return Result<ProjectDto>.Success(dto);
        }
        catch (Exception ex)
        {
            await _uow.RollbackTransactionAsync(cancellationToken);
            _logger.Error(ex, "Failed to update project {ProjectId}", request.Id);
            return Result<ProjectDto>.Error(_localizer[MessageKeys.Error.Project.UpdateFailed]);
        }
    }

    public async Task<Result<bool>> AddFeaturedProjectAsync(AddFeaturedProjectCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Updating featured flag for project {ProjectId} to {IsFeatured}", request.ProjectId, request.IsFeatured);

        var existingProject = await _uow.Repository<Project>()
           .FirstOrDefaultAsync(c => c.Id.ToString() == request.ProjectId && !c.IsDeleted, cancellationToken);

        if (existingProject is null)
        {
            _logger.Warn("Project {ProjectId} was not found for featured update", request.ProjectId);
            return Result<bool>.Error(_localizer[MessageKeys.Error.Project.NotFound]);
        }

        existingProject.IsFeatured = request.IsFeatured;

        await _uow.Repository<Project>().UpdateAsync(existingProject, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        _logger.Info("Updated featured flag for project {ProjectId} to {IsFeatured}", request.ProjectId, request.IsFeatured);
        return Result<bool>.Success(true);
    }


    private static void ApplyTranslations(List<ProjectDto> projects, string lang)
    {
        foreach (var dto in projects)
            ApplyTranslations(dto, lang);
    }

    private static void ApplyTranslations(ProjectDto dto, string lang)
    {
        if (dto is null) return;

        var translation = dto.ProjectTranslations?
            .FirstOrDefault(t => t.LanguageCode == lang)
            ?? dto.ProjectTranslations?
            .FirstOrDefault(t => t.LanguageCode == SupportedLanguages.DefaultCode);

        if (translation is null) return;

        dto.Name = translation.Name ?? dto.Name;
        dto.Description = translation.Description ?? dto.Description;
        dto.Facilities = translation.Facilities ?? dto.Facilities;
    }
    private async Task AddProjectImagesAsync(Project project, List<ProjectImagesCommand> images, CancellationToken cancellationToken)
    {
        var folderName = $"{project.Id}";
        var folderPath = Path.Combine("wwwroot", "images", "projects", folderName);
        Directory.CreateDirectory(folderPath);

        var primaryImageName = images.FirstOrDefault(i => i.IsPrimary)?.Name;

        var projectImages = new List<ProjectImage>();

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
                _logger.Warn("Invalid base64 data for project image {ImageName} on project {ProjectId}", image.Name, project.Id);
                continue;
            }

            var diskPath = Path.Combine(folderPath, image.Name);
            var relativePath = Path.Combine("images", "projects", folderName, image.Name);

            await using (var fileStream = new FileStream(diskPath, FileMode.Create))
            {
                await fileStream.WriteAsync(imageData, 0, imageData.Length);
            }

            projectImages.Add(new ProjectImage
            {
                Project = project,
                ImageName = image.Name,
                ImagePath = relativePath,
                IsPrimary = image.Name == primaryImageName
            });
        }

        // Fallback: mark first image as primary if none was flagged
        if (projectImages.Count > 0 && !projectImages.Any(i => i.IsPrimary))
            projectImages[0].IsPrimary = true;

        await _uow.Repository<ProjectImage>().AddRangeAsync(projectImages, cancellationToken);
        _logger.Info("Added {ImageCount} image(s) to project {ProjectId}", projectImages.Count, project.Id);
    }

    private async Task UpdateProjectImagesAsync(Project project, List<ProjectImagesCommand> images, CancellationToken cancellationToken)
    {
        var folderName = $"{project.Id}";
        var folderPath = Path.Combine("wwwroot", "images", "projects", folderName);
        Directory.CreateDirectory(folderPath);

        // Get all existing non-deleted images for this car
        var existingImages = await _uow.Repository<ProjectImage>()
            .AsQueryable()
            .Where(i => i.Project!.Id == project.Id && !i.IsDeleted)
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
                    _logger.Info("Deleted old project image file {ImagePath} for project {ProjectId}", imageToDelete.ImagePath, project.Id);
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
                    _logger.Warn("Project image {ImageName} was not found in database for project {ProjectId}", image.Name, project.Id);
                    continue;
                }

                existingImage.IsPrimary = isPrimary;
                await _uow.Repository<ProjectImage>().UpdateAsync(existingImage, cancellationToken);
                continue;
            }

            byte[] imageData;
            try
            {
                imageData = Convert.FromBase64String(image.Data);
            }
            catch (FormatException)
            {
                _logger.Warn("Invalid base64 data for project image {ImageName} on project {ProjectId}", image.Name, project.Id);
                continue;
            }

            var diskPath = Path.Combine(folderPath, image.Name);
            var relativePath = Path.Combine("images", "projects", folderName, image.Name);

            if (existingImage is not null)
            {
                // Delete old file from disk
                if (!string.IsNullOrEmpty(existingImage.ImagePath))
                {
                    var oldFilePath = Path.Combine("wwwroot", existingImage.ImagePath);
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                        _logger.Info("Deleted replaced project image file {ImagePath} for project {ProjectId}", existingImage.ImagePath, project.Id);
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

                await _uow.Repository<ProjectImage>().UpdateAsync(existingImage, cancellationToken);
            }
            else
            {
                await using (var fileStream = new FileStream(diskPath, FileMode.Create))
                {
                    await fileStream.WriteAsync(imageData, 0, imageData.Length);
                }

                var newImage = new ProjectImage
                {
                    Project = project,
                    ImageName = image.Name,
                    ImagePath = relativePath,
                    IsPrimary = isPrimary
                };

                await _uow.Repository<ProjectImage>().AddAsync(newImage, cancellationToken);
            }
        }

        _logger.Info("Updated project images for project {ProjectId}", project.Id);
    }


    private async Task SaveProjectFloorPlanAsync(Project project, string? base64Data, string? fileName, CancellationToken cancellationToken)
    {
        // Delete old file if exists
        if (!string.IsNullOrEmpty(project.ProjectPlanFileUrl))
        {
            var oldFilePath = Path.Combine("wwwroot", project.ProjectPlanFileUrl);
            if (File.Exists(oldFilePath))
            {
                File.Delete(oldFilePath);
                _logger.Info("Deleted old project plan file {FilePath} for project {ProjectId}", project.ProjectPlanFileUrl, project.Id);
            }

            project.ProjectPlanFileUrl = null;
            project.ProjectPlanFileName = null;
        }

        if (string.IsNullOrEmpty(base64Data) || string.IsNullOrEmpty(fileName))
            return;

        // ✅ Strip the data URL prefix if present (e.g. "data:application/pdf;base64,")
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
            _logger.Warn("Invalid base64 PDF data for plan on project {ProjectId}", project.Id);
            return;
        }

        var folderPath = Path.Combine("wwwroot", "images", "projects", $"{project.Id}", "plans");
        Directory.CreateDirectory(folderPath);

        var relativePath = Path.Combine("images", "projects", $"{project.Id}", "plans", fileName);
        var diskPath = Path.Combine("wwwroot", relativePath);

        await using var fs = new FileStream(diskPath, FileMode.Create);
        await fs.WriteAsync(pdfData, 0, pdfData.Length, cancellationToken);

        project.ProjectPlanFileName = fileName;
        project.ProjectPlanFileUrl  = relativePath;

        _logger.Info("Saved project plan {FileName} for project {ProjectId}", fileName, project.Id);
    }
}

