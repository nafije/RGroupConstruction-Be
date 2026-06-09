using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Layout.Commands.CreateLayout;
using RGroupConstruction.Application.Features.Layout.Commands.DeleteLayout;
using RGroupConstruction.Application.Features.Layout.Commands.UpdateLayout;
using RGroupConstruction.Application.Features.Layout.Queries.GetAllLayouts;
using RGroupConstruction.Application.Features.Layout.Queries.GetAllPagedLayouts;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace RGroupConstruction.Application.Services;

public class LayoutService(IUnitOfWork _uow, IMapper _mapper, ILogger<LayoutService> _logger, IMessageLocalizer _localizer) : ILayoutService
{
    public async Task<Result<LayoutDto>> CreateLayoutAsync(CreateLayoutCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Creating layout {LayoutName}", request.Name);

        if (request.Name is not null)
        {
            var existingLayout = await _uow.Repository<Layout>()
                .FirstOrDefaultAsync(c => c.Name == request.Name, cancellationToken);

            if (existingLayout is not null)
            {
                _logger.Warn("Layout {LayoutName} already exists", request.Name);
                return Result<LayoutDto>.Error(_localizer[MessageKeys.Error.Layout.LayoutExists]);
            }
        }

        try
        {
            var newLayout = new Layout
            {
                Name = request.Name
            };

            await _uow.Repository<Layout>().AddAsync(newLayout, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            _logger.Info("Created layout {LayoutId}", newLayout.Id);
            return Result<LayoutDto>.Success(_mapper.Map<LayoutDto>(newLayout));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to create layout {LayoutName}", request.Name);
            return Result<LayoutDto>.Error(_localizer[MessageKeys.Error.Layout.CreateFailed]);
        }
    }

    public async Task<Result<bool>> DeleteLayoutAsync(DeleteLayoutCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Deleting layout {LayoutId}", request.Id);

        var existingLayout = await _uow.Repository<Layout>()
           .FirstOrDefaultAsync(c => c.Id.ToString() == request.Id && !c.IsDeleted, cancellationToken);

        if (existingLayout is null)
        {
            _logger.Warn("Layout {LayoutId} was not found for delete", request.Id);
            return Result<bool>.Error(_localizer[MessageKeys.Error.Layout.NotFound]);
        }

        existingLayout.IsDeleted = true;

        await _uow.Repository<Layout>().UpdateAsync(existingLayout, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        _logger.Info("Deleted layout {LayoutId}", request.Id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<IEnumerable<LayoutDto>>> GetAllLayoutsAsync(GetAllLayoutsQuery request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Getting all layouts");

        var layouts = await _uow.Repository<Layout>()
           .AsQueryable()
           .Where(c => !c.IsDeleted)
           .ToListAsync(cancellationToken);

        var mapped = _mapper.Map<IEnumerable<LayoutDto>>(layouts);
        _logger.Info("Retrieved {LayoutCount} layouts", layouts.Count);
        return Result.Success(mapped);
    }

    public async Task<Result<PagedResponse<LayoutDto>>> GetAllPagedLayoutsAsync(GetAllPagedLayoutsQuery request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Getting paged layouts page {PageNumber} with size {PageSize} and search {Search}", request.PageNr, request.PageSize, request.Search);

        var query = _uow.Repository<Layout>()
            .AsQueryable()
            .Where(c => !c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(c => c.Name != null && c.Name.ToLower().Contains(request.Search.ToLower()));

        var totalCount = await query.CountAsync(cancellationToken);

        var layouts = await query
            .Skip((request.PageNr - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<LayoutDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var pagedResponse = new PagedResponse<LayoutDto>(layouts, totalCount, request.PageNr, request.PageSize);
        _logger.Info("Retrieved {LayoutCount} layouts from total {TotalCount}", layouts.Count, totalCount);
        return Result<PagedResponse<LayoutDto>>.Success(pagedResponse);
    }

    public async Task<Result<LayoutDto>> UpdateLayoutAsync(UpdateLayoutCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Updating layout {LayoutId}", request.Id);

        var existingLayout = await _uow.Repository<Layout>()
            .FirstOrDefaultAsync(c => c.Id.ToString() == request.Id, cancellationToken);

        if (existingLayout is null)
        {
            _logger.Warn("Layout {LayoutId} was not found for update", request.Id);
            return Result<LayoutDto>.Error(_localizer[MessageKeys.Error.Layout.NotFound]);
        }

        if (request.Name is not null)
        {
            var duplicateLayout = await _uow.Repository<Layout>()
                .FirstOrDefaultAsync(c => c.Name == request.Name && c.Id.ToString() != request.Id, cancellationToken);

            if (duplicateLayout is not null)
            {
                _logger.Warn("Layout name {LayoutName} already exists for another layout", request.Name);
                return Result<LayoutDto>.Error(_localizer[MessageKeys.Error.Layout.LayoutExists]);
            }
        }

        try
        {
            existingLayout.Name = request.Name;

            await _uow.Repository<Layout>().UpdateAsync(existingLayout, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            _logger.Info("Updated layout {LayoutId}", request.Id);
            return Result<LayoutDto>.Success(_mapper.Map<LayoutDto>(existingLayout));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to update layout {LayoutId}", request.Id);
            return Result<LayoutDto>.Error(_localizer[MessageKeys.Error.Layout.UpdateFailed]);
        }
    }
}


