using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Ads.Commands.DeleteAds;

internal class DeleteAdsCommandHandler(IAdsService _adsService) : ICommandHandler<DeleteAdsCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteAdsCommand request, CancellationToken cancellationToken)
        => await _adsService.DeleteAsync(request, cancellationToken);
}

