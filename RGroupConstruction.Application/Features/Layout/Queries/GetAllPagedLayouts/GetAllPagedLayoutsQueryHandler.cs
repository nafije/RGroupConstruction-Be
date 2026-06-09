using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Layout.Queries.GetAllPagedLayouts;

internal class GetAllPagedLayoutsQueryHandler(ILayoutService _layoutService) : IQueryHandler<GetAllPagedLayoutsQuery, PagedResponse<LayoutDto>>
{
    public async Task<Result<PagedResponse<LayoutDto>>> Handle(GetAllPagedLayoutsQuery request, CancellationToken cancellationToken)
        => await _layoutService.GetAllPagedLayoutsAsync(request, cancellationToken);
}


