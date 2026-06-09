using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Ads.Queries.GetAllAds;

internal class GetAllAdsQueryHandler(IAdsService _adsService) : IQueryHandler<GetAllAdsQuery, IEnumerable<AdsDto>>
{
    public async Task<Result<IEnumerable<AdsDto>>> Handle(GetAllAdsQuery request, CancellationToken cancellationToken)
        => await _adsService.GetAllAsync(request, cancellationToken);
}

