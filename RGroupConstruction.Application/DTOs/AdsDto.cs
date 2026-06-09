using RGroupConstruction.Application.DTOs.Base;

namespace RGroupConstruction.Application.DTOs;

public class AdsDto : BaseDto<Guid>
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ImageName { get; set; }
    public string? ImageUrl { get; set; }
    public string? VideoName { get; set; }
    public string? VideoUrl { get; set; }
    public string? LinkUrl { get; set; }
    public bool IsActive { get; set; }
    public List<AdsTranslationDto>? AdsTranslations { get; set; }
}

