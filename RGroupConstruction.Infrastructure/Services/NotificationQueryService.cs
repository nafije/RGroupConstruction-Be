using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Common.Events;
using RGroupConstruction.Infrastructure.Hubs;
using RGroupConstruction.Infrastructure.Presistence;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace RGroupConstruction.Infrastructure.Services;

public class NotificationQueryService(
    ApplicationDbContext dbContext,
    IMapper mapper,
    IHubContext<NotificationHub> hubContext,
    IMessageLocalizer _localizer) : INotificationQueryService
{
    public async Task<Result<bool>> AdminDeleteNotificationAsync(
       Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await dbContext.UserNotifications
            .Include(n => n.User)
            .FirstOrDefaultAsync(n => n.Id == notificationId && !n.IsDeleted && (n.User == null || !n.User.IsDeleted), cancellationToken);

        if (notification is null)
            return Result<bool>.Error(_localizer[MessageKeys.Error.Notification.NotFoundOrForbidden]);

        notification.IsDeleted = true;
        await dbContext.SaveChangesAsync(cancellationToken);

        if (notification.User is not null)
            await hubContext.Clients.User(notification.User.Id).SendAsync(SignalREvents.NotificationDeleted, new
            {
                notification.Id
            }, cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> AdminMarkAsReadAsync(
        Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await dbContext.UserNotifications
            .Include(n => n.User)
            .FirstOrDefaultAsync(n => n.Id == notificationId && !n.IsDeleted && (n.User == null || !n.User.IsDeleted), cancellationToken);

        if (notification is null)
            return Result<bool>.Error(_localizer[MessageKeys.Error.Notification.NotFoundOrForbidden]);

        notification.IsRead = true;
        await dbContext.SaveChangesAsync(cancellationToken);

        if (notification.User is not null)
            await hubContext.Clients.User(notification.User.Id).SendAsync(SignalREvents.NotificationRead, new
            {
                notification.Id
            }, cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<PagedResponse<UserNotificationDto>>> GetAllNotificationsAsync(
       Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = dbContext.UserNotifications
            .Include(n => n.User)
            .Where(n => n.User != null && n.User.Id == userId.ToString() && !n.User.IsDeleted && !n.IsDeleted)
            .OrderByDescending(n => n.CreatedOn);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var mapped = mapper.Map<List<UserNotificationDto>>(items);
        return Result<PagedResponse<UserNotificationDto>>.Success(
            new PagedResponse<UserNotificationDto>(mapped, totalCount, pageNumber, pageSize));
    }

    public async Task<Result<int>> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var count = await dbContext.UserNotifications
            .Include(n => n.User)
            .Where(n => n.User != null && n.User.Id == userId.ToString() && !n.User.IsDeleted && !n.IsRead && !n.IsDeleted)
            .CountAsync(cancellationToken);

        return Result<int>.Success(count);
    }

    public async Task<Result<bool>> MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var notifications = await dbContext.UserNotifications
            .Include(n => n.User)
            .Where(n => n.User != null && n.User.Id == userId.ToString() && !n.User.IsDeleted && !n.IsRead && !n.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var notification in notifications)
            notification.IsRead = true;

        await dbContext.SaveChangesAsync(cancellationToken);

        await hubContext.Clients.User(userId.ToString()).SendAsync(SignalREvents.AllNotificationsRead,
            cancellationToken);

        return Result<bool>.Success(true);
    }
}

