using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Application.Commands.CreateJobApplication;
using RGroupConstruction.Application.Features.Application.Queries;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Entities;
using RGroupConstruction.Domain.Enums;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace RGroupConstruction.Application.Services;

public class JobApplicationService(
    IUnitOfWork _uow,
    IMapper _mapper,
    ILogger<JobApplicationService> _logger,
    IMessageLocalizer _localizer,
    IEmailService _emailService,
    INotificationService _notificationService,
    UserManager<User> _userManager) : IJobApplicationService
{
    public async Task<Result<bool>> CreateJobApplicationAsync(CreateJobApplicationCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Creating job application for job {JobId} from {ApplicantEmail}", request.JobId, request.Email);

        var job = await _uow.Repository<Job>()
            .FirstOrDefaultAsync(j => j.Id.ToString() == request.JobId && !j.IsDeleted, cancellationToken);

        if (job is null)
        {
            _logger.Warn("Job {JobId} was not found while creating job application for {ApplicantEmail}", request.JobId, request.Email);
            return Result<bool>.Error(_localizer[MessageKeys.Error.JobApplication.JobNotFound]);
        }

        try
        {
            var newApplication = new JobApplication
            {
                Job = job,
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                Message = request.Message,
                CvFileUrl = request.CvFileUrl,
                CvOriginalFileName = request.CvOriginalFileName,
            };

            await _uow.Repository<JobApplication>().AddAsync(newApplication, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            await SaveCvFileAsync(newApplication, request.CvFileUrl, request.CvOriginalFileName, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            _logger.Info("Created job application {JobApplicationId} for job {JobId}", newApplication.Id, job.Id);

            var notificationMessage = $"{newApplication.FullName} applied for '{job.Title}'";

            var notificationResult = await _notificationService.SendNotificationToAdminsAsync(
                message: notificationMessage,
                type: UserNotificationType.NewJobApplication,
                cancellationToken: cancellationToken);

            if (!notificationResult.IsSuccessful)
                _logger.Warn("Failed to save job application notifications for job {JobId}", job.Id);

            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");

            if (adminUsers.Count > 0)
            {
                var usersWithEmail = adminUsers
                    .Where(u => !string.IsNullOrEmpty(u.Email))
                    .ToList();

                var tasks = usersWithEmail.Select(u => _emailService.SendJobApplicationToAdminAsync(
                    adminEmail: u.Email!,
                    applicantFullName: newApplication.FullName ?? string.Empty,
                    applicantEmail: newApplication.Email ?? string.Empty,
                    applicantPhone: newApplication.Phone ?? string.Empty,
                    jobTitle: job.Title ?? string.Empty,
                    message: newApplication.Message ?? string.Empty,
                    cancellationToken: cancellationToken));

                var results = await Task.WhenAll(tasks);

                foreach (var (result, user) in results.Zip(usersWithEmail))
                {
                    if (!result.IsSuccessful)
                        _logger.Warn("Failed to send job application notification to admin {AdminUserId} for application {JobApplicationId}", user.Id, newApplication.Id);
                }
            }
            else
                _logger.Warn("No admin users found for job application {JobApplicationId} notification", newApplication.Id);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to create job application.");
            return Result<bool>.Error(_localizer[MessageKeys.Error.JobApplication.CreateFailed]);
        }
    }

    public async Task<Result<PagedResponse<JobApplicationDto>>> GetAllPagedJobApplicationsAsync(GetAllPagedJobApplicationsQuery request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Getting paged job applications page {PageNumber} with size {PageSize} and search {Search}", request.PageNr, request.PageSize, request.Search);

        var query = _uow.Repository<JobApplication>()
            .AsQueryable()
            .Include(a => a.Job)
            .Where(a => !a.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(a =>
                (a.FullName != null && a.FullName.ToLower().Contains(request.Search.ToLower())) ||
                (a.Email != null && a.Email.ToLower().Contains(request.Search.ToLower())) ||
                (a.Job!.Title != null && a.Job.Title.ToLower().Contains(request.Search.ToLower())));

        if (!string.IsNullOrWhiteSpace(request.JobId))
            query = query.Where(j => j.Job!.Id.ToString() == request.JobId);

        var totalCount = await query.CountAsync(cancellationToken);

        var applications = await query
            .Skip((request.PageNr - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var mapped = _mapper.Map<List<JobApplicationDto>>(applications);

        var pagedResponse = new PagedResponse<JobApplicationDto>(mapped, totalCount, request.PageNr, request.PageSize);
        _logger.Info("Retrieved {JobApplicationCount} job applications from total {TotalCount}", mapped.Count, totalCount);
        return Result<PagedResponse<JobApplicationDto>>.Success(pagedResponse);
    }

    private async Task SaveCvFileAsync(JobApplication application, string? base64Data, string? fileName, CancellationToken cancellationToken)
    {
        // Delete old file if exists
        if (!string.IsNullOrEmpty(application.CvFileUrl))
        {
            var oldFilePath = Path.Combine("wwwroot", application.CvFileUrl);
            if (File.Exists(oldFilePath))
            {
                File.Delete(oldFilePath);
                _logger.Info("Deleted old unit floor plan file {FilePath} for application {ApplicationId}", application.CvFileUrl, application.Id);
            }

            application.CvFileUrl = null;
            application.CvOriginalFileName = null;
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
            _logger.Warn("Invalid base64 PDF data for floor plan on unit {ApplicationId}", application.Id);
            return;
        }

        var folderPath = Path.Combine("wwwroot", "images", "application", $"{application.Id}", "cv");
        Directory.CreateDirectory(folderPath);

        var relativePath = Path.Combine("images", "application", $"{application.Id}", "cv", fileName);
        var diskPath = Path.Combine("wwwroot", relativePath);

        await using var fs = new FileStream(diskPath, FileMode.Create);
        await fs.WriteAsync(pdfData, 0, pdfData.Length, cancellationToken);

        application.CvOriginalFileName = fileName;
        application.CvFileUrl  = relativePath;

        _logger.Info("Saved unit floor plan {FileName} for unit {ApplicationId}", fileName, application.Id);
    }
}

