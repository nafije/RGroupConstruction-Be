using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;

namespace RGroupConstruction.Application.Features.Ads.Queries.GetAllAds;

public class GetAllAdsQuery : IQuery<IEnumerable<AdsDto>>;
