namespace RGroupConstruction.Domain.Common.Events;

public static class SignalREvents
{
    public const string ReceiveNotification = nameof(ReceiveNotification);
    public const string NotificationRead = nameof(NotificationRead);
    public const string NotificationUnread = nameof(NotificationUnread);
    public const string NotificationDeleted = nameof(NotificationDeleted);
    public const string AllNotificationsRead = nameof(AllNotificationsRead);
}

