using RGroupConstruction.Application.Common.Paging;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;

namespace RGroupConstruction.Application.Features.Ads.Queries.GetAllPagedAds;

public class GetAllPagedAdsQuery : PagedQuery, IQuery<PagedResponse<AdsDto>> { }

