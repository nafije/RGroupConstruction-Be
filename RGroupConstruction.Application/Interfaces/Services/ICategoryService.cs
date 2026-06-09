using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Category.Commands.CreateCategory;
using RGroupConstruction.Application.Features.Category.Commands.DeleteCategory;
using RGroupConstruction.Application.Features.Category.Commands.UpdateCategory;
using RGroupConstruction.Application.Features.Category.Queries.GetAllCategories;
using RGroupConstruction.Application.Features.Category.Queries.GetAllPagedCategories;

namespace RGroupConstruction.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<Result<CategoryDto>> CreateCategoryAsync(CreateCategoryCommand request, CancellationToken cancellationToken = default);
    Task<Result<CategoryDto>> UpdateCategoryAsync(UpdateCategoryCommand request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteCategoryAsync(DeleteCategoryCommand request, CancellationToken cancellationToken = default);
    Task<Result<PagedResponse<CategoryDto>>> GetAllPagedCategoriesAsync(GetAllPagedCategoriesQuery request, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<CategoryDto>>> GetAllCategoriesAsync(GetAllCategoriesQuery request, CancellationToken cancellationToken = default);
}

