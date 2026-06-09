using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Notifications.Commands.MarkAllNotificationsRead;

internal class MarkAllNotificationsReadCommandHandler(INotificationQueryService _notificationQueryService)
    : ICommandHandler<MarkAllNotificationsReadCommand, bool>
{
    public async Task<Result<bool>> Handle(MarkAllNotificationsReadCommand request, CancellationToken cancellationToken)
        => await _notificationQueryService.MarkAllAsReadAsync(request.UserId, cancellationToken);
}

