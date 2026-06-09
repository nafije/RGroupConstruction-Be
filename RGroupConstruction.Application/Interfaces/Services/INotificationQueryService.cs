using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;

namespace RGroupConstruction.Application.Interfaces.Services;

public interface INotificationQueryService
{
    Task<Result<PagedResponse<UserNotificationDto>>> GetAllNotificationsAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<Result<int>> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> AdminMarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default);
    Task<Result<bool>> MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> AdminDeleteNotificationAsync(Guid notificationId, CancellationToken cancellationToken = default);
}

