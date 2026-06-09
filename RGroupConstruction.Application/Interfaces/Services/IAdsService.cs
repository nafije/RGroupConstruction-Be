using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Ads.Commands.CreateAds;
using RGroupConstruction.Application.Features.Ads.Commands.DeleteAds;
using RGroupConstruction.Application.Features.Ads.Commands.UpdateAds;
using RGroupConstruction.Application.Features.Ads.Queries.GetAllAds;
using RGroupConstruction.Application.Features.Ads.Queries.GetAllPagedAds;

namespace RGroupConstruction.Application.Interfaces.Services;

public interface IAdsService
{
    Task<Result<AdsDto>> CreateAsync(CreateAdsCommand request, CancellationToken cancellationToken = default);
    Task<Result<AdsDto>> UpdateAsync(UpdateAdsCommand request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(DeleteAdsCommand request, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<AdsDto>>> GetAllAsync(GetAllAdsQuery request, CancellationToken cancellationToken = default);
    Task<Result<PagedResponse<AdsDto>>> GetAllPagedAsync(GetAllPagedAdsQuery request, CancellationToken cancellationToken = default);
}

