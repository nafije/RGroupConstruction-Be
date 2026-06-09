using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Common.Model;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Job.Commands.CreateJob;
using RGroupConstruction.Application.Features.Job.Commands.DeleteJob;
using RGroupConstruction.Application.Features.Job.Commands.UpdateJob;
using RGroupConstruction.Application.Features.Job.Commands.UpdateJobActivityStatus;
using RGroupConstruction.Application.Features.Job.Queries.GetAllActiveJobs;
using RGroupConstruction.Application.Features.Job.Queries.GetAllJobs;
using RGroupConstruction.Application.Features.Job.Queries.GetAllPagedJobs;
using RGroupConstruction.Application.Features.Job.Queries.GetJobById;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Common.Constants;
using RGroupConstruction.Domain.Entities;
using RGroupConstruction.Domain.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace RGroupConstruction.Application.Services;

public class JObService(
    IUnitOfWork _uow,
    IMapper _mapper,
    ILogger<JObService> _logger,
    IMessageLocalizer _localizer,
    IEmailService _emailService) : IJobService
{
    public async Task<Result<bool>> UpdateJobActivityStatusAsync(
        UpdateJobActivityStatusCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.Info("Updating job {JobId} activity status to {IsActive}", request.JobId, request.IsActive);

        var existingJob = await _uow.Repository<Job>()
            .FirstOrDefaultAsync(c => c.Id.ToString() == request.JobId && !c.IsDeleted, cancellationToken);

        if (existingJob is null)
        {
            _logger.Warn("Job {JobId} was not found for activity status update", request.JobId);
            return Result<bool>.Error(_localizer[MessageKeys.Error.Job.NotFound]);
        }

        try
        {
            existingJob.IsActive = request.IsActive;

            await _uow.Repository<Job>().UpdateAsync(existingJob, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            _logger.Info("Updated job {JobId} activity status to {IsActive}", request.JobId, request.IsActive);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to update activity status for job {JobId}", request.JobId);
            return Result<bool>.Error(_localizer[MessageKeys.Error.Job.UpdateFailed]);
        }
    }

    public async Task<Result<JobDto>> CreateJobAsync(
        CreateJobCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.Info("Creating job {JobTitle}", request.Title);

        if (request.Title is not null)
        {
            var existingJob = await _uow.Repository<Job>()
                .FirstOrDefaultAsync(c => c.Title == request.Title, cancellationToken);

            if (existingJob is not null)
            {
                _logger.Warn("Job {JobTitle} already exists", request.Title);
                return Result<JobDto>.Error(_localizer[MessageKeys.Error.Job.JobExists]);
            }
        }

        var department = await _uow.Repository<Department>()
            .FirstOrDefaultAsync(p => p.Id.ToString() == request.DepartmentId && !p.IsDeleted, cancellationToken);

        if (department is null)
        {
            _logger.Warn("Department {DepartmentId} was not found while creating job {JobTitle}", request.DepartmentId, request.Title);
            return Result<JobDto>.Error(_localizer[MessageKeys.Error.Job.DepartmentNotFound]);
        }

        Enum.TryParse<EmploymentType>(request.EmploymentType, out var type);

        try
        {
            var newJob = new Job
            {
                Title = GetDefaultTitle(request.Title, request.JobTranslations),
                Requirements = GetDefaultRequirements(request.Requirements, request.JobTranslations),
                Department = department,
                EmploymentType = type,
                Location = request.Location,
                SalaryFrom = request.SalaryFrom,
                SalaryTo = request.SalaryTo,
                IsActive = request.IsActive,
                ExpiresAt = request.ExpiresAt
            };

            await _uow.Repository<Job>().AddAsync(newJob, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            await SyncJobTranslationsAsync(
                newJob,
                request.Title,
                request.Requirements,
                request.JobTranslations,
                cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);

            _logger.Info("Created job {JobId}", newJob.Id);

            var subscriberEmails = await _uow.Repository<Subscribe>()
                .AsQueryable()
                .Where(s => !s.IsDeleted)
                .Select(s => s.Email!)
                .ToListAsync(cancellationToken);

            if (subscriberEmails.Count > 0)
            {
                _logger.Info("Sending new-job emails for job {JobId} to {SubscriberCount} subscriber(s)", newJob.Id, subscriberEmails.Count);

                var tasks = subscriberEmails.Select(email =>
                    _emailService.SendNewJobEmailToUserAsync(
                        jobTitle: newJob.Title ?? string.Empty,
                        department: department.Name ?? string.Empty,
                        location: newJob.Location ?? string.Empty,
                        employmentType: newJob.EmploymentType.ToString(),
                        userEmail: email,
                        jobId: newJob.Id,
                        cancellationToken: cancellationToken));

                var results = await Task.WhenAll(tasks);

                foreach (var (result, email) in results.Zip(subscriberEmails))
                {
                    if (!result.IsSuccessful)
                        _logger.Warn("Failed to send new-job email to {SubscriberEmail} for job {JobId}", email, newJob.Id);
                }
            }
            else
                _logger.Info("No subscribers found for new-job emails for job {JobId}", newJob.Id);

            return Result<JobDto>.Success(
                MapToDto(newJob, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to create job {JobTitle}", request.Title);
            return Result<JobDto>.Error(_localizer[MessageKeys.Error.Job.CreateFailed]);
        }
    }

    public async Task<Result<bool>> DeleteJobAsync(
        DeleteJobCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.Info("Deleting job {JobId}", request.Id);

        var existingJob = await _uow.Repository<Job>()
            .AsQueryable()
            .Include(j => j.JobTranslations!.Where(t => !t.IsDeleted))
            .FirstOrDefaultAsync(c => c.Id.ToString() == request.Id && !c.IsDeleted, cancellationToken);

        if (existingJob is null)
        {
            _logger.Warn("Job {JobId} was not found for delete", request.Id);
            return Result<bool>.Error(_localizer[MessageKeys.Error.Job.NotFound]);
        }

        existingJob.IsDeleted = true;

        foreach (var translation in existingJob.JobTranslations ?? [])
            translation.IsDeleted = true;

        await _uow.Repository<Job>().UpdateAsync(existingJob, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        _logger.Info("Deleted job {JobId}", request.Id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<IEnumerable<JobDto>>> GetAllJobsAsync(
        GetAllJobsQuery request,
        CancellationToken cancellationToken = default)
    {
        _logger.Info("Getting all jobs");

        var jobs = await _uow.Repository<Job>()
            .AsQueryable()
            .Include(j => j.Department)
            .Include(j => j.JobTranslations!.Where(t => !t.IsDeleted))
            .Where(c => !c.IsDeleted)
            .ToListAsync(cancellationToken);

        var languageCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var mapped = jobs.Select(j => MapToDto(j, languageCode));

        _logger.Info("Retrieved {JobCount} jobs", jobs.Count);
        return Result.Success(mapped);
    }

    public async Task<Result<PagedResponse<JobDto>>> GetAllPagedJobsAsync(
     GetAllPagedJobsQuery request,
     CancellationToken cancellationToken = default)
    {
        _logger.Info(
            "Getting paged jobs page {PageNumber} with size {PageSize}, search {Search}, department {DepartmentId}, employment type {EmploymentType}",
            request.PageNr,
            request.PageSize,
            request.Search,
            request.DepartmentId,
            request.EmploymentType);

        var query = _uow.Repository<Job>()
            .AsQueryable()
            .Include(j => j.Department)
            .Include(j => j.JobTranslations!.Where(t => !t.IsDeleted))
            .Where(j => !j.IsDeleted);

        query = ApplyJobFilters(
            query,
            request.Search,
            request.DepartmentId,
            request.EmploymentType,
            request.SalaryFrom,
            request.SalaryTo);

        var totalCount = await query.CountAsync(cancellationToken);

        var jobsWithApplicants = await query
            .Skip((request.PageNr - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(j => new
            {
                Job = j,
                TotalApplicants = j.Applications!.Count(a => !a.IsDeleted)
            })
            .ToListAsync(cancellationToken);

        var languageCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        var mapped = jobsWithApplicants.Select(x =>
        {
            var dto = MapToDto(x.Job, languageCode);
            dto.TotalApplicants = x.TotalApplicants;
            return dto;
        }).ToList();

        var pagedResponse = new PagedResponse<JobDto>(
            mapped,
            totalCount,
            request.PageNr,
            request.PageSize);

        _logger.Info(
            "Retrieved {JobCount} jobs from total {TotalCount}",
            mapped.Count,
            totalCount);

        return Result<PagedResponse<JobDto>>.Success(pagedResponse);
    }

    public async Task<Result<JobDto>> GetJobByIdAsync(
        GetJobByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        _logger.Info("Getting job by id {JobId}", request.Id);

        var totalApplicants = await _uow.Repository<Job>()
            .AsQueryable()
            .Where(j => j.Id.ToString() == request.Id && !j.IsDeleted)
            .CountAsync(cancellationToken);

        var job = await _uow.Repository<Job>()
            .AsQueryable()
            .Include(j => j.Department)
            .Include(j => j.JobTranslations!.Where(t => !t.IsDeleted))
            .FirstOrDefaultAsync(j => j.Id.ToString() == request.Id && !j.IsDeleted, cancellationToken);

        if (job is null)
        {
            _logger.Warn("Job with id {JobId} not found", request.Id);
            return Result<JobDto>.Error("Job not found");
        }

        var languageCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var dto = MapToDto(job, languageCode);
        dto.TotalApplicants = totalApplicants;

        _logger.Info("Retrieved job with id {JobId}", request.Id);
        return Result<JobDto>.Success(dto);
    }

    public async Task<Result<PagedResponse<JobDto>>> GetAllActiveJobsAsync(
        GetAllActiveJobsQuery request,
        CancellationToken cancellationToken = default)
    {
        _logger.Info("Getting active jobs page {PageNumber} with size {PageSize}, search {Search}, department {DepartmentId}, employment type {EmploymentType}", request.PageNr, request.PageSize, request.Search, request.DepartmentId, request.EmploymentType);

        var query = _uow.Repository<Job>()
            .AsQueryable()
            .Include(j => j.Department)
            .Include(j => j.JobTranslations!.Where(t => !t.IsDeleted))
            .Where(c => !c.IsDeleted && c.IsActive && c.ExpiresAt > DateTime.UtcNow);

        query = ApplyJobFilters(
            query,
            request.Search,
            request.DepartmentId,
            request.EmploymentType,
            request.SalaryFrom,
            request.SalaryTo);

        var totalCount = await query.CountAsync(cancellationToken);

        var jobs = await query
            .Skip((request.PageNr - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var languageCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var mapped = jobs.Select(j => MapToDto(j, languageCode)).ToList();

        var pagedResponse = new PagedResponse<JobDto>(mapped, totalCount, request.PageNr, request.PageSize);
        _logger.Info("Retrieved {JobCount} active jobs from total {TotalCount}", mapped.Count, totalCount);
        return Result<PagedResponse<JobDto>>.Success(pagedResponse);
    }

    public async Task<Result<JobDto>> UpdateJobAsync(
        UpdateJobCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.Info("Updating job {JobId}", request.Id);

        var existingJob = await _uow.Repository<Job>()
            .AsQueryable()
            .Include(j => j.JobTranslations!.Where(t => !t.IsDeleted))
            .FirstOrDefaultAsync(c => c.Id.ToString() == request.Id && !c.IsDeleted, cancellationToken);

        if (existingJob is null)
        {
            _logger.Warn("Job {JobId} was not found for update", request.Id);
            return Result<JobDto>.Error(_localizer[MessageKeys.Error.Job.NotFound]);
        }

        if (request.Title is not null)
        {
            var duplicateJob = await _uow.Repository<Job>()
                .FirstOrDefaultAsync(c => c.Title == request.Title && c.Id.ToString() != request.Id, cancellationToken);

            if (duplicateJob is not null)
            {
                _logger.Warn("Job title {JobTitle} already exists for another job", request.Title);
                return Result<JobDto>.Error(_localizer[MessageKeys.Error.Job.JobExists]);
            }
        }

        var department = await _uow.Repository<Department>()
            .FirstOrDefaultAsync(p => p.Id.ToString() == request.DepartmentId && !p.IsDeleted, cancellationToken);

        if (department is null)
        {
            _logger.Warn("Department {DepartmentId} was not found while updating job {JobId}", request.DepartmentId, request.Id);
            return Result<JobDto>.Error(_localizer[MessageKeys.Error.Job.DepartmentNotFound]);
        }

        Enum.TryParse<EmploymentType>(request.EmploymentType, out var type);

        try
        {
            existingJob.Title = GetDefaultTitle(request.Title, request.JobTranslations) ?? existingJob.Title;
            existingJob.Requirements = GetDefaultRequirements(request.Requirements, request.JobTranslations) ?? existingJob.Requirements;
            existingJob.Department = department;
            existingJob.EmploymentType = type;
            existingJob.Location = request.Location;
            existingJob.SalaryFrom = request.SalaryFrom;
            existingJob.SalaryTo = request.SalaryTo;
            existingJob.IsActive = request.IsActive;
            existingJob.ExpiresAt = request.ExpiresAt;

            await SyncJobTranslationsAsync(
                existingJob,
                request.Title,
                request.Requirements,
                request.JobTranslations,
                cancellationToken);

            await _uow.Repository<Job>().UpdateAsync(existingJob, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            _logger.Info("Updated job {JobId}", request.Id);
            return Result<JobDto>.Success(
                MapToDto(existingJob, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to update job {JobId}", request.Id);
            return Result<JobDto>.Error(_localizer[MessageKeys.Error.Job.UpdateFailed]);
        }
    }

    private async Task SyncJobTranslationsAsync(
        Job job,
        string? fallbackTitle,
        string? fallbackRequirements,
        IEnumerable<JobTranslationCommand>? requestedTranslations,
        CancellationToken cancellationToken)
    {
        var translationsByLanguage = BuildTranslationValues(
            fallbackTitle, fallbackRequirements, requestedTranslations);

        if (translationsByLanguage.Count == 0)
            return;

        job.JobTranslations ??= [];

        foreach (var (languageCode, values) in translationsByLanguage)
        {
            var (title, requirements) = values;

            var existing = job.JobTranslations.FirstOrDefault(
                t => t.LanguageCode == languageCode && !t.IsDeleted);

            if (existing is not null)
            {
                existing.Title = title;
                existing.Requirements = requirements;
                await _uow.Repository<JobTranslation>().UpdateAsync(existing, cancellationToken);
                continue;
            }

            var newTranslation = new JobTranslation
            {
                Job = job,
                LanguageCode = languageCode,
                Title = title,
                Requirements = requirements
            };

            job.JobTranslations.Add(newTranslation);
            await _uow.Repository<JobTranslation>().AddAsync(newTranslation, cancellationToken);
        }
    }

    private static Dictionary<string, (string Title, string Requirements)> BuildTranslationValues(
        string? fallbackTitle,
        string? fallbackRequirements,
        IEnumerable<JobTranslationCommand>? requestedTranslations)
    {
        var values = new Dictionary<string, (string, string)>(StringComparer.OrdinalIgnoreCase);

        foreach (var translation in requestedTranslations ?? [])
        {
            var languageCode = translation.LanguageCode?.ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(languageCode) ||
                !SupportedLanguages.IsSupported(languageCode) ||
                (string.IsNullOrWhiteSpace(translation.Title) && string.IsNullOrWhiteSpace(translation.Requirements)))
                continue;

            values[languageCode] = (
                translation.Title ?? string.Empty,
                translation.Requirements ?? string.Empty);
        }

        var defaultTitle = GetDefaultTitle(fallbackTitle, requestedTranslations);
        var defaultRequirements = GetDefaultRequirements(fallbackRequirements, requestedTranslations);

        foreach (var languageCode in SupportedLanguages.Codes)
            values.TryAdd(languageCode, (
                defaultTitle ?? string.Empty,
                defaultRequirements ?? string.Empty));

        return values;
    }

    private static string? GetDefaultTitle(
        string? fallbackTitle,
        IEnumerable<JobTranslationCommand>? requestedTranslations)
    {
        if (!string.IsNullOrWhiteSpace(fallbackTitle))
            return fallbackTitle;

        var defaultTranslation = requestedTranslations?.FirstOrDefault(t =>
            t.LanguageCode?.Equals(SupportedLanguages.DefaultCode, StringComparison.OrdinalIgnoreCase) == true);

        return defaultTranslation?.Title
            ?? requestedTranslations?.FirstOrDefault(t => !string.IsNullOrWhiteSpace(t.Title))?.Title;
    }

    private static string? GetDefaultRequirements(
        string? fallbackRequirements,
        IEnumerable<JobTranslationCommand>? requestedTranslations)
    {
        if (!string.IsNullOrWhiteSpace(fallbackRequirements))
            return fallbackRequirements;

        var defaultTranslation = requestedTranslations?.FirstOrDefault(t =>
            t.LanguageCode?.Equals(SupportedLanguages.DefaultCode, StringComparison.OrdinalIgnoreCase) == true);

        return defaultTranslation?.Requirements
            ?? requestedTranslations?.FirstOrDefault(t => !string.IsNullOrWhiteSpace(t.Requirements))?.Requirements;
    }

    private static JobDto MapToDto(Job job, string languageCode)
    {
        var translations = job.JobTranslations?
            .Where(t => !t.IsDeleted)
            .ToList() ?? [];

        var translation = translations.FirstOrDefault(t => t.LanguageCode == languageCode)
                       ?? translations.FirstOrDefault(t => t.LanguageCode == SupportedLanguages.DefaultCode);

        return new JobDto
        {
            Id = job.Id,
            CreatedOn = job.CreatedOn,
            ModifiedOn = job.ModifiedOn,
            Title = translation?.Title ?? job.Title,
            Requirements = translation?.Requirements ?? job.Requirements,
            Location = job.Location,
            SalaryFrom = job.SalaryFrom,
            SalaryTo = job.SalaryTo,
            IsActive = job.IsActive,
            ExpiresAt = job.ExpiresAt,
            EmploymentType = job.EmploymentType.ToString(),
            Department = job.Department is not null ? new DepartmentDto
            {
                Id = job.Department.Id,
                Name = job.Department.Name
            } : null,
            JobTranslations = translations.Select(t => new JobTranslationDto
            {
                LanguageCode = t.LanguageCode,
                Title = t.Title,
                Requirements = t.Requirements
            }).ToList()
        };
    }

    private static IQueryable<Job> ApplyJobFilters(
        IQueryable<Job> query,
        string? search,
        string? departmentId,
        string? employmentType,
        decimal? salaryFrom,
        decimal? salaryTo)
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            var lower = search.ToLower();
            query = query.Where(c =>
                (c.Title != null && c.Title.ToLower().Contains(lower)) ||
                c.JobTranslations!.Any(t => t.Title != null && t.Title.ToLower().Contains(lower)));
        }

        if (!string.IsNullOrWhiteSpace(departmentId))
            query = query.Where(c => c.Department!.Id.ToString() == departmentId);

        if (!string.IsNullOrWhiteSpace(employmentType))
            query = query.Where(c => c.EmploymentType.ToString() == employmentType);

        if (salaryFrom.HasValue)
            query = query.Where(c => c.SalaryFrom >= salaryFrom.Value);

        if (salaryTo.HasValue)
            query = query.Where(c => c.SalaryTo <= salaryTo.Value);

        return query;
    }

}

