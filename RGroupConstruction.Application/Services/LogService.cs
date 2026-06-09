using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Log.Queries.GetAllPagedLogs;
using RGroupConstruction.Application.Features.Log.Queries.GetLogById;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace RGroupConstruction.Application.Services;

public class LogService(
    IUnitOfWork unitOfWork,
    ILogger<LogService> logger) : ILogService
{
    public async Task<Result<PagedResponse<LogEntryDto>>> GetAllPagedLogsAsync(
        GetAllPagedLogsQuery request,
        CancellationToken cancellationToken = default)
    {
        logger.Info(
            "Getting paged logs page {PageNumber} with size {PageSize}, search {Search}, level {Level}, from {FromDate}, to {ToDate}",
            request.PageNr,
            request.PageSize,
            request.Search,
            request.Level,
            request.FromDate,
            request.ToDate);

        var query = unitOfWork.Repository<LogEntry>()
            .AsQueryable()
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Level))
        {
            var level = request.Level.Trim().ToLower();
            query = query.Where(log => log.Level != null && log.Level.ToLower() == level);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(log =>
                (log.Message != null && log.Message.ToLower().Contains(search)) ||
                (log.Template != null && log.Template.ToLower().Contains(search)) ||
                (log.Exception != null && log.Exception.ToLower().Contains(search)) ||
                (log.Properties != null && log.Properties.ToLower().Contains(search)));
        }

        if (request.FromDate.HasValue)
            query = query.Where(log => log.CreatedAt >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(log => log.CreatedAt <= request.ToDate.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var logs = await query
            .OrderByDescending(log => log.CreatedAt)
            .ThenByDescending(log => log.Id)
            .Skip((request.PageNr - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(log => new LogEntryDto
            {
                Id = log.Id,
                Timestamp = log.Timestamp,
                Level = log.Level,
                Template = log.Template,
                Message = log.Message,
                Exception = log.Exception,
                Properties = log.Properties,
                CreatedAt = log.CreatedAt
            })
            .ToListAsync(cancellationToken);

        logger.Info("Retrieved {LogCount} logs from total {TotalCount}", logs.Count, totalCount);
        return Result<PagedResponse<LogEntryDto>>.Success(
            new PagedResponse<LogEntryDto>(logs, totalCount, request.PageNr, request.PageSize));
    }

    public async Task<Result<LogEntryDto>> GetLogByIdAsync(
        GetLogByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        logger.Info("Getting log {LogId}", request.Id);

        var log = await unitOfWork.Repository<LogEntry>()
            .AsQueryable()
            .AsNoTracking()
            .Where(log => log.Id == request.Id)
            .Select(log => new LogEntryDto
            {
                Id = log.Id,
                Timestamp = log.Timestamp,
                Level = log.Level,
                Template = log.Template,
                Message = log.Message,
                Exception = log.Exception,
                Properties = log.Properties,
                CreatedAt = log.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (log is null)
        {
            logger.Warn("Log {LogId} was not found", request.Id);
            return Result<LogEntryDto>.Error("Log not found.");
        }

        return Result<LogEntryDto>.Success(log);
    }
}

