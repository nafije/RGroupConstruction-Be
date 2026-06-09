using RGroupConstruction.Domain.Common;

namespace RGroupConstruction.Domain.Entities;

public class AdsTranslation : AuditedEntity<Guid>
{
    public Ads? Ads { get; set; }
    public string? LanguageCode { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
}

