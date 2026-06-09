using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Category.Queries.GetAllCategories;

internal class GetAllCategoriesQueryHandler(ICategoryService _categoryService) : IQueryHandler<GetAllCategoriesQuery, IEnumerable<CategoryDto>>
{
    public async Task<Result<IEnumerable<CategoryDto>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        => await _categoryService.GetAllCategoriesAsync(request, cancellationToken);
}

