using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Ads.Commands.CreateAds;

internal class CreateAdsCommandHandler(IAdsService _adsService) : ICommandHandler<CreateAdsCommand, AdsDto>
{
    public async Task<Result<AdsDto>> Handle(CreateAdsCommand request, CancellationToken cancellationToken)
        => await _adsService.CreateAsync(request, cancellationToken);
}

