using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;

namespace RGroupConstruction.Application.Features.Category.Queries.GetAllCategories;

public class GetAllCategoriesQuery : IQuery<IEnumerable<CategoryDto>>;

