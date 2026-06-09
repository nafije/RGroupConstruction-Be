using RGroupConstruction.Application.Interfaces.Query;

namespace RGroupConstruction.Application.Features.Notifications.Queries.GetUnreadCount;

public class GetUnreadCountQuery : IQuery<int>
{
    public Guid UserId { get; set; }
}

