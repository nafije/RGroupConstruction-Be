using RGroupConstruction.Domain.Common;

namespace RGroupConstruction.Domain.Entities;

public class JobTranslation : AuditedEntity<Guid>
{
    public Job? Job { get; set; }
    public string? LanguageCode { get; set; }
    public string? Title { get; set; }
    public string? Requirements { get; set; }
}

