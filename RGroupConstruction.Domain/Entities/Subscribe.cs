using RGroupConstruction.Domain.Common;

namespace RGroupConstruction.Domain.Entities;

public class Subscribe : AuditedEntity<Guid>
{
    public string? Email { get; set; }
    public bool IsUnsubscribed { get; set; }
}

