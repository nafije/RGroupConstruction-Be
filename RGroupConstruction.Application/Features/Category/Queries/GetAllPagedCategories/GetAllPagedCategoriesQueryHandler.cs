using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Category.Queries.GetAllPagedCategories;

internal class GetAllPagedCategoriesQueryHandler(ICategoryService _categoryService) : IQueryHandler<GetAllPagedCategoriesQuery, PagedResponse<CategoryDto>>
{
    public async Task<Result<PagedResponse<CategoryDto>>> Handle(GetAllPagedCategoriesQuery request, CancellationToken cancellationToken)
        => await _categoryService.GetAllPagedCategoriesAsync(request, cancellationToken);
}

