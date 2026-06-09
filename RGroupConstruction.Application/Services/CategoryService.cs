using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Category.Commands.CreateCategory;
using RGroupConstruction.Application.Features.Category.Commands.DeleteCategory;
using RGroupConstruction.Application.Features.Category.Commands.UpdateCategory;
using RGroupConstruction.Application.Features.Category.Queries.GetAllCategories;
using RGroupConstruction.Application.Features.Category.Queries.GetAllPagedCategories;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace RGroupConstruction.Application.Services;

public class CategoryService(IUnitOfWork _uow, IMapper _mapper, ILogger<CategoryService> _logger, IMessageLocalizer _localizer) : ICategoryService
{
    public async Task<Result<CategoryDto>> CreateCategoryAsync(CreateCategoryCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Creating category {CategoryName}", request.Name);

        if (request.Name is not null)
        {
            var existingCategory = await _uow.Repository<Category>()
                .FirstOrDefaultAsync(c => c.Name == request.Name, cancellationToken);

            if (existingCategory is not null)
            {
                _logger.Warn("Category {CategoryName} already exists", request.Name);
                return Result<CategoryDto>.Error(_localizer[MessageKeys.Error.Category.CategoryExists]);
            }
        }

        try
        {
            var newCategory = new Category
            {
                Name = request.Name
            };

            await _uow.Repository<Category>().AddAsync(newCategory, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            _logger.Info("Created category {CategoryId}", newCategory.Id);
            return Result<CategoryDto>.Success(_mapper.Map<CategoryDto>(newCategory));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to create category {CategoryName}", request.Name);
            return Result<CategoryDto>.Error(_localizer[MessageKeys.Error.Category.CreateFailed]);
        }
    }

    public async Task<Result<bool>> DeleteCategoryAsync(DeleteCategoryCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Deleting category {CategoryId}", request.Id);

        var existingCategory = await _uow.Repository<Category>()
           .FirstOrDefaultAsync(c => c.Id.ToString() == request.Id && !c.IsDeleted, cancellationToken);

        if (existingCategory is null)
        {
            _logger.Warn("Category {CategoryId} was not found for delete", request.Id);
            return Result<bool>.Error(_localizer[MessageKeys.Error.Category.NotFound]);
        }

        existingCategory.IsDeleted = true;

        await _uow.Repository<Category>().UpdateAsync(existingCategory, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        _logger.Info("Deleted category {CategoryId}", request.Id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<IEnumerable<CategoryDto>>> GetAllCategoriesAsync(GetAllCategoriesQuery request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Getting all categories");

        var categories = await _uow.Repository<Category>()
           .AsQueryable()
           .Where(c => !c.IsDeleted)
           .ToListAsync(cancellationToken);

        var mapped = _mapper.Map<IEnumerable<CategoryDto>>(categories);
        _logger.Info("Retrieved {CategoryCount} categories", categories.Count);
        return Result.Success(mapped);
    }

    public async Task<Result<PagedResponse<CategoryDto>>> GetAllPagedCategoriesAsync(GetAllPagedCategoriesQuery request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Getting paged categories page {PageNumber} with size {PageSize} and search {Search}", request.PageNr, request.PageSize, request.Search);

        var query = _uow.Repository<Category>()
            .AsQueryable()
            .Where(c => !c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(c => c.Name != null && c.Name.ToLower().Contains(request.Search.ToLower()));

        var totalCount = await query.CountAsync(cancellationToken);

        var categories = await query
            .Skip((request.PageNr - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var pagedResponse = new PagedResponse<CategoryDto>(categories, totalCount, request.PageNr, request.PageSize);
        _logger.Info("Retrieved {CategoryCount} categories from total {TotalCount}", categories.Count, totalCount);
        return Result<PagedResponse<CategoryDto>>.Success(pagedResponse);
    }

    public async Task<Result<CategoryDto>> UpdateCategoryAsync(UpdateCategoryCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Updating category {CategoryId}", request.Id);

        var existingCategory = await _uow.Repository<Category>()
            .FirstOrDefaultAsync(c => c.Id.ToString() == request.Id, cancellationToken);

        if (existingCategory is null)
        {
            _logger.Warn("Category {CategoryId} was not found for update", request.Id);
            return Result<CategoryDto>.Error(_localizer[MessageKeys.Error.Category.NotFound]);
        }

        if (request.Name is not null)
        {
            var duplicateLayout = await _uow.Repository<Category>()
                .FirstOrDefaultAsync(c => c.Name == request.Name && c.Id.ToString() != request.Id, cancellationToken);

            if (duplicateLayout is not null)
            {
                _logger.Warn("Category name {CategoryName} already exists for another category", request.Name);
                return Result<CategoryDto>.Error(_localizer[MessageKeys.Error.Category.CategoryExists]);
            }
        }

        try
        {
            existingCategory.Name = request.Name;

            await _uow.Repository<Category>().UpdateAsync(existingCategory, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            _logger.Info("Updated category {CategoryId}", request.Id);
            return Result<CategoryDto>.Success(_mapper.Map<CategoryDto>(existingCategory));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to update category {CategoryId}", request.Id);
            return Result<CategoryDto>.Error(_localizer[MessageKeys.Error.Category.UpdateFailed]);
        }
    }
}

