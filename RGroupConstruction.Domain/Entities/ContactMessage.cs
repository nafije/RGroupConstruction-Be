using RGroupConstruction.Domain.Common;

namespace RGroupConstruction.Domain.Entities;

public class ContactMessage : AuditedEntity<Guid>
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Subject { get; set; }
    public string? Message { get; set; }
}


