using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Notifications.Queries.GetUnreadCount;

internal class GetUnreadCountQueryHandler(INotificationQueryService _notificationQueryService)
  : IQueryHandler<GetUnreadCountQuery, int>
{
    public async Task<Result<int>> Handle(GetUnreadCountQuery request, CancellationToken cancellationToken)
        => await _notificationQueryService.GetUnreadCountAsync(request.UserId, cancellationToken);
}

