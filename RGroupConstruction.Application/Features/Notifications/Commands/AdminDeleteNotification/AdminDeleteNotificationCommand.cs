using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Notifications.Commands.AdminDeleteNotification;


public class AdminDeleteNotificationCommand : ICommand<bool>
{
    public Guid NotificationId { get; set; }
}

