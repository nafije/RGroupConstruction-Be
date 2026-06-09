using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Notifications.Commands.AdminMarkNotificationRead;

public class AdminMarkNotificationReadCommand : ICommand<bool>
{
    public Guid NotificationId { get; set; }
}

