using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Ads.Queries.GetAllPagedAds;

internal class GetAllPagedAdsQueryHandler(IAdsService _adsService) : IQueryHandler<GetAllPagedAdsQuery, PagedResponse<AdsDto>>
{
    public async Task<Result<PagedResponse<AdsDto>>> Handle(GetAllPagedAdsQuery request, CancellationToken cancellationToken)
        => await _adsService.GetAllPagedAsync(request, cancellationToken);
}

