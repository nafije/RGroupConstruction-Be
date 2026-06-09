using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Notifications.Commands.MarkAllNotificationsRead;

public class MarkAllNotificationsReadCommand : ICommand<bool>
{
    public Guid UserId { get; set; }
}

