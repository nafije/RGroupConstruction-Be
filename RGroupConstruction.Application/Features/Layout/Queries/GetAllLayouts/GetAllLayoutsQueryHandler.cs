using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Layout.Queries.GetAllLayouts;

internal class GetAllLayoutsQueryHandler(ILayoutService _layoutService) : IQueryHandler<GetAllLayoutsQuery, IEnumerable<LayoutDto>>
{
    public async Task<Result<IEnumerable<LayoutDto>>> Handle(GetAllLayoutsQuery request, CancellationToken cancellationToken)
        => await _layoutService.GetAllLayoutsAsync(request, cancellationToken);
}

