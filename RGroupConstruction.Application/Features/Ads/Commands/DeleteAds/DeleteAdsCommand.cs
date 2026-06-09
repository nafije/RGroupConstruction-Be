using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Ads.Commands.DeleteAds;

public class DeleteAdsCommand : ICommand<bool>
{
    public string? Id { get; set; }
}

