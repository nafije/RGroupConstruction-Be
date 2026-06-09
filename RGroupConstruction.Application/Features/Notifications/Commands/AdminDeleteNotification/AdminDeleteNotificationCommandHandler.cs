using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Notifications.Commands.AdminDeleteNotification;

internal class AdminDeleteNotificationCommandHandler(INotificationQueryService _notificationQueryService)
    : ICommandHandler<AdminDeleteNotificationCommand, bool>
{
    public async Task<Result<bool>> Handle(AdminDeleteNotificationCommand request, CancellationToken cancellationToken)
        => await _notificationQueryService.AdminDeleteNotificationAsync(request.NotificationId, cancellationToken);
}

