using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Notifications.Commands.AdminMarkNotificationRead;

internal class AdminMarkNotificationReadCommandHandler(INotificationQueryService _notificationQueryService)
    : ICommandHandler<AdminMarkNotificationReadCommand, bool>
{
    public async Task<Result<bool>> Handle(AdminMarkNotificationReadCommand request, CancellationToken cancellationToken)
        => await _notificationQueryService.AdminMarkAsReadAsync(request.NotificationId, cancellationToken);
}

