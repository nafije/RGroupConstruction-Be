using RGroupConstruction.Domain.Common;

namespace RGroupConstruction.Domain.Entities;

public class Status : AuditedEntity<Guid>
{
    public string? Name { get; set; }
}


