using RGroupConstruction.Application.DTOs;

namespace RGroupConstruction.Application.Features.Notifications.Queries.GetAdminNotifications;

public class GetAdminNotificationsQuery : PageNumberPagedQuery<UserNotificationDto>
{
    public Guid UserId { get; set; }
}


