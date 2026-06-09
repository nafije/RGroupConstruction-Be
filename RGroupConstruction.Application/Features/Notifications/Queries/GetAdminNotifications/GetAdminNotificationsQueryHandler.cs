using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Notifications.Queries.GetAdminNotifications;

internal class GetAdminNotificationsQueryHandler(INotificationQueryService _notificationQueryService)
    : IQueryHandler<GetAdminNotificationsQuery, PagedResponse<UserNotificationDto>>
{
    public async Task<Result<PagedResponse<UserNotificationDto>>> Handle(
        GetAdminNotificationsQuery request, CancellationToken cancellationToken)
        => await _notificationQueryService.GetAllNotificationsAsync(request.UserId, request.PageNumber, request.PageSize, cancellationToken);
}

