using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Department.Commands.CreateDepartment;
using RGroupConstruction.Application.Features.Department.Commands.DeleteDepartment;
using RGroupConstruction.Application.Features.Department.Commands.UpdateDepartment;
using RGroupConstruction.Application.Features.Department.Queries.GetAllDepartments;
using RGroupConstruction.Application.Features.Department.Queries.GetAllPagedDepartments;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace RGroupConstruction.Application.Services;

public class DepartmentService(IUnitOfWork _uow, IMapper _mapper, ILogger<DepartmentService> _logger, IMessageLocalizer _localizer) : IDepartmentService
{
    public async Task<Result<DepartmentDto>> CreateDepartmentAsync(CreateDepartmentCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Creating department {DepartmentName}", request.Name);

        if (request.Name is not null)
        {
            var existingDepartment = await _uow.Repository<Department>()
                .FirstOrDefaultAsync(c => c.Name == request.Name, cancellationToken);

            if (existingDepartment is not null)
            {
                _logger.Warn("Department {DepartmentName} already exists", request.Name);
                return Result<DepartmentDto>.Error(_localizer[MessageKeys.Error.Department.DepartmentExists]);
            }
        }

        try
        {
            var newDepartment = new Department
            {
                Name = request.Name
            };

            await _uow.Repository<Department>().AddAsync(newDepartment, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            _logger.Info("Created department {DepartmentId}", newDepartment.Id);
            return Result<DepartmentDto>.Success(_mapper.Map<DepartmentDto>(newDepartment));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to create department {DepartmentName}", request.Name);
            return Result<DepartmentDto>.Error(_localizer[MessageKeys.Error.Department.CreateFailed]);
        }
    }

    public async Task<Result<bool>> DeleteDepartmentAsync(DeleteDepartmentCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Deleting department {DepartmentId}", request.Id);

        var existingDepartment = await _uow.Repository<Department>()
           .FirstOrDefaultAsync(c => c.Id.ToString() == request.Id && !c.IsDeleted, cancellationToken);

        if (existingDepartment is null)
        {
            _logger.Warn("Department {DepartmentId} was not found for delete", request.Id);
            return Result<bool>.Error(_localizer[MessageKeys.Error.Department.NotFound]);
        }

        existingDepartment.IsDeleted = true;

        await _uow.Repository<Department>().UpdateAsync(existingDepartment, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        _logger.Info("Deleted department {DepartmentId}", request.Id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<IEnumerable<DepartmentDto>>> GetAllDepartmentsAsync(GetAllDepartmentsQuery request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Getting all departments");

        var departments = await _uow.Repository<Department>()
           .AsQueryable()
           .Where(c => !c.IsDeleted)
           .ToListAsync(cancellationToken);

        var mapped = _mapper.Map<IEnumerable<DepartmentDto>>(departments);
        _logger.Info("Retrieved {DepartmentCount} departments", departments.Count);
        return Result.Success(mapped);
    }

    public async Task<Result<PagedResponse<DepartmentDto>>> GetAllPagedDepartmentsAsync(GetAllPagedDepartmentsQuery request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Getting paged departments page {PageNumber} with size {PageSize} and search {Search}", request.PageNr, request.PageSize, request.Search);

        var query = _uow.Repository<Department>()
            .AsQueryable()
            .Where(c => !c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(c => c.Name != null && c.Name.ToLower().Contains(request.Search.ToLower()));

        var totalCount = await query.CountAsync(cancellationToken);

        var departments = await query
            .Skip((request.PageNr - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<DepartmentDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var pagedResponse = new PagedResponse<DepartmentDto>(departments, totalCount, request.PageNr, request.PageSize);
        _logger.Info("Retrieved {DepartmentCount} departments from total {TotalCount}", departments.Count, totalCount);
        return Result<PagedResponse<DepartmentDto>>.Success(pagedResponse);
    }

    public async Task<Result<DepartmentDto>> UpdateDepartmentAsync(UpdateDepartmentCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Updating department {DepartmentId}", request.Id);

        var existingDepartment = await _uow.Repository<Department>()
            .FirstOrDefaultAsync(c => c.Id.ToString() == request.Id, cancellationToken);

        if (existingDepartment is null)
        {
            _logger.Warn("Department {DepartmentId} was not found for update", request.Id);
            return Result<DepartmentDto>.Error(_localizer[MessageKeys.Error.Department.NotFound]);
        }

        if (request.Name is not null)
        {
            var duplicateDepartment = await _uow.Repository<Department>()
                .FirstOrDefaultAsync(c => c.Name == request.Name && c.Id.ToString() != request.Id, cancellationToken);

            if (duplicateDepartment is not null)
            {
                _logger.Warn("Department name {DepartmentName} already exists for another department", request.Name);
                return Result<DepartmentDto>.Error(_localizer[MessageKeys.Error.Department.DepartmentExists]);
            }
        }

        try
        {
            existingDepartment.Name = request.Name;

            await _uow.Repository<Department>().UpdateAsync(existingDepartment, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            _logger.Info("Updated department {DepartmentId}", request.Id);
            return Result<DepartmentDto>.Success(_mapper.Map<DepartmentDto>(existingDepartment));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to update department {DepartmentId}", request.Id);
            return Result<DepartmentDto>.Error(_localizer[MessageKeys.Error.Department.UpdateFailed]);
        }
    }
}

