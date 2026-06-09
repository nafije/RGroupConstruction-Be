using RGroupConstruction.Domain.Common;
using RGroupConstruction.Domain.Enums;

namespace RGroupConstruction.Domain.Entities;

public class UserNotification : AuditedEntity<Guid>
{
    public string? UserId { get; set; }
    public required virtual User User { get; set; }
    public string? Message { get; set; }
    public bool IsRead { get; set; }
    public required UserNotificationType UserNotificationType { get; set; }
}

