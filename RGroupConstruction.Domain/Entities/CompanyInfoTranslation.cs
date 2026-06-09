using RGroupConstruction.Domain.Common;

namespace RGroupConstruction.Domain.Entities;

public class CompanyInfoTranslation : AuditedEntity<Guid>
{
    public CompanyInfo? CompanyInfo { get; set; }
    public string? LanguageCode { get; set; }
    public string? Name { get; set; }
    public string? Tagline { get; set; }
    public string? AboutText { get; set; }
    public string? MissionTitle { get; set; }
    public string? MissionDescription { get; set; }
    public string? WhyChooseUs { get; set; }
}

