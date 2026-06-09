using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Status.Commands.CreateStatus;
using RGroupConstruction.Application.Features.Status.Commands.DeleteStatus;
using RGroupConstruction.Application.Features.Status.Commands.UpdateStatus;
using RGroupConstruction.Application.Features.Status.Queries.GetAllPagedStatuses;
using RGroupConstruction.Application.Features.Status.Queries.GetAllStatuses;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace RGroupConstruction.Application.Services;

public class StatusService(IUnitOfWork _uow, IMapper _mapper, ILogger<StatusService> _logger, IMessageLocalizer _localizer) : IStatusService
{
    public async Task<Result<StatusDto>> CreateStatusAsync(CreateStatusCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Creating status {StatusName}", request.Name);

        if (request.Name is not null)
        {
            var existingStatus = await _uow.Repository<Status>()
                .FirstOrDefaultAsync(c => c.Name == request.Name, cancellationToken);

            if (existingStatus is not null)
            {
                _logger.Warn("Status {StatusName} already exists", request.Name);
                return Result<StatusDto>.Error(_localizer[MessageKeys.Error.Status.StatusExists]);
            }
        }

        try
        {
            var newStatus = new Status
            {
                Name = request.Name
            };

            await _uow.Repository<Status>().AddAsync(newStatus, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            _logger.Info("Created status {StatusId}", newStatus.Id);
            return Result<StatusDto>.Success(_mapper.Map<StatusDto>(newStatus));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to create status {StatusName}", request.Name);
            return Result<StatusDto>.Error(_localizer[MessageKeys.Error.Status.CreateFailed]);
        }
    }

    public async Task<Result<bool>> DeleteStatusAsync(DeleteStatusCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Deleting status {StatusId}", request.Id);

        var existingStatus = await _uow.Repository<Status>()
           .FirstOrDefaultAsync(c => c.Id.ToString() == request.Id && !c.IsDeleted, cancellationToken);

        if (existingStatus is null)
        {
            _logger.Warn("Status {StatusId} was not found for delete", request.Id);
            return Result<bool>.Error(_localizer[MessageKeys.Error.Status.NotFound]);
        }

        existingStatus.IsDeleted = true;

        await _uow.Repository<Status>().UpdateAsync(existingStatus, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        _logger.Info("Deleted status {StatusId}", request.Id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<IEnumerable<StatusDto>>> GetAllStatusesAsync(GetAllStatusesQuery request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Getting all statuses");

        var statuses = await _uow.Repository<Status>()
           .AsQueryable()
           .Where(c => !c.IsDeleted)
           .ToListAsync(cancellationToken);

        var mapped = _mapper.Map<IEnumerable<StatusDto>>(statuses);
        _logger.Info("Retrieved {StatusCount} statuses", statuses.Count);
        return Result.Success(mapped);
    }

    public async Task<Result<PagedResponse<StatusDto>>> GetAllPagedStatusesAsync(GetAllPagedStatusesQuery request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Getting paged statuses page {PageNumber} with size {PageSize} and search {Search}", request.PageNr, request.PageSize, request.Search);

        var query = _uow.Repository<Status>()
            .AsQueryable()
            .Where(c => !c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(c => c.Name != null && c.Name.ToLower().Contains(request.Search.ToLower()));

        var totalCount = await query.CountAsync(cancellationToken);

        var statuses = await query
            .Skip((request.PageNr - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<StatusDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var pagedResponse = new PagedResponse<StatusDto>(statuses, totalCount, request.PageNr, request.PageSize);
        _logger.Info("Retrieved {StatusCount} statuses from total {TotalCount}", statuses.Count, totalCount);
        return Result<PagedResponse<StatusDto>>.Success(pagedResponse);
    }

    public async Task<Result<StatusDto>> UpdateStatusAsync(UpdateStatusCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Updating status {StatusId}", request.Id);

        var existingStatus = await _uow.Repository<Status>()
            .FirstOrDefaultAsync(c => c.Id.ToString() == request.Id, cancellationToken);

        if (existingStatus is null)
        {
            _logger.Warn("Status {StatusId} was not found for update", request.Id);
            return Result<StatusDto>.Error(_localizer[MessageKeys.Error.Status.NotFound]);
        }

        if (request.Name is not null)
        {
            var duplicateStatus = await _uow.Repository<Status>()
                .FirstOrDefaultAsync(c => c.Name == request.Name && c.Id.ToString() != request.Id, cancellationToken);

            if (duplicateStatus is not null)
            {
                _logger.Warn("Status name {StatusName} already exists for another status", request.Name);
                return Result<StatusDto>.Error(_localizer[MessageKeys.Error.Status.StatusExists]);
            }
        }

        try
        {
            existingStatus.Name = request.Name;

            await _uow.Repository<Status>().UpdateAsync(existingStatus, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            _logger.Info("Updated status {StatusId}", request.Id);
            return Result<StatusDto>.Success(_mapper.Map<StatusDto>(existingStatus));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to update status {StatusId}", request.Id);
            return Result<StatusDto>.Error(_localizer[MessageKeys.Error.Status.UpdateFailed]);
        }
    }

}

