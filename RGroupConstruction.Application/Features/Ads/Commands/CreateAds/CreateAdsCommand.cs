using RGroupConstruction.Application.Common.Model;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Ads.Commands.CreateAds;

public class CreateAdsCommand : ICommand<AdsDto>
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public List<AdsTranslationCommand> AdsTranslations { get; set; } = [];
    public string? ImageData { get; set; }
    public string? ImageName { get; set; }
    public string? VideoName { get; set; }
    public string? VideoData { get; set; }
    public string? LinkUrl { get; set; }
    public bool IsActive { get; set; }
}
