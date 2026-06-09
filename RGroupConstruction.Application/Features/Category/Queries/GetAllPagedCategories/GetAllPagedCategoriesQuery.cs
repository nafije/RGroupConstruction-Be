using RGroupConstruction.Application.Common.Paging;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;

namespace RGroupConstruction.Application.Features.Category.Queries.GetAllPagedCategories;

public class GetAllPagedCategoriesQuery : PagedQuery, IQuery<PagedResponse<CategoryDto>> { }


