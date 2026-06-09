using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Common.Events;
using RGroupConstruction.Domain.Entities;
using RGroupConstruction.Domain.Enums;
using RGroupConstruction.Infrastructure.Hubs;
using RGroupConstruction.Infrastructure.Presistence;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace RGroupConstruction.Infrastructure.Services;


public class NotificationService(
    IHubContext<NotificationHub> hubContext,
    ApplicationDbContext dbContext,
    ILogger<NotificationService> logger,
    IMessageLocalizer _localizer) : INotificationService
{
    public async Task<Result<bool>> SendNotificationToAdminsAsync(string message, UserNotificationType type, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.Info("Sending admin notification of type {NotificationType}", type);

            var adminUsers = await dbContext.Users
                .Where(u => !u.IsDeleted && dbContext.UserRoles
                    .Any(ur => ur.UserId == u.Id && dbContext.Roles
                        .Any(r => r.Id == ur.RoleId && r.Name == UserRole.Admin.ToString() && !r.IsDeleted)))
                .ToListAsync(cancellationToken);

            if (!adminUsers.Any())
            {
                logger.Info("No admin users found for notification type {NotificationType}", type);
                return Result<bool>.Success(true);
            }

            var notifications = new List<UserNotification>();
            foreach (var admin in adminUsers)
            {
                notifications.Add(new UserNotification
                {
                    User = admin,
                    Message = message,
                    UserNotificationType = type,
                    IsRead = false,
                    CreatedOn = DateTime.UtcNow
                });
            }

            await dbContext.UserNotifications.AddRangeAsync(notifications, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            logger.Info(
                "Saved {NotificationCount} admin notification(s) of type {NotificationType}",
                notifications.Count,
                type);

            foreach (var notification in notifications)
            {
                await hubContext.Clients.User(notification.User.Id).SendAsync(SignalREvents.ReceiveNotification, new
                {
                    notification.Id,
                    notification.Message,
                    notification.UserNotificationType,
                    notification.CreatedOn,
                    notification.IsRead
                }, cancellationToken);
            }

            logger.Info(
                "Sent {NotificationCount} SignalR admin notification(s) of type {NotificationType}",
                notifications.Count,
                type);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Failed to send admin notification of type {NotificationType}", type);
            return Result<bool>.Error(_localizer[MessageKeys.Error.Notification.AdminFailed]);
        }
    }
}


