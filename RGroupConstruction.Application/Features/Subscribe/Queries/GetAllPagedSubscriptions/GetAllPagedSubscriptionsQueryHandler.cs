using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Subscribe.Queries.GetAllPagedSubscriptions;

internal class GetAllPagedSubscriptionsQueryHandler(ISubscribeService _subscribeService) : IQueryHandler<GetAllPagedSubscriptionsQuery, PagedResponse<SubscribeDto>>
{
    public async Task<Result<PagedResponse<SubscribeDto>>> Handle(GetAllPagedSubscriptionsQuery request, CancellationToken cancellationToken)
        => await _subscribeService.GetALlPagedSubscriptionsAsync(request, cancellationToken);
}

