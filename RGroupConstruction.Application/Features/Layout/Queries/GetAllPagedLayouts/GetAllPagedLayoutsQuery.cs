using RGroupConstruction.Application.Common.Paging;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;

namespace RGroupConstruction.Application.Features.Layout.Queries.GetAllPagedLayouts;

public class GetAllPagedLayoutsQuery : PagedQuery, IQuery<PagedResponse<LayoutDto>> { }
