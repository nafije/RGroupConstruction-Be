using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Ads.Commands.UpdateAds;

internal class UpdateAdsCommandHandler(IAdsService _adsService) : ICommandHandler<UpdateAdsCommand, AdsDto>
{
    public async Task<Result<AdsDto>> Handle(UpdateAdsCommand request, CancellationToken cancellationToken)
        => await _adsService.UpdateAsync(request, cancellationToken);
}


