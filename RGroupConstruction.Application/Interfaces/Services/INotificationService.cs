using RGroupConstruction.Application.Common;
using RGroupConstruction.Domain.Enums;

namespace RGroupConstruction.Application.Interfaces.Services;

public interface INotificationService
{
    Task<Result<bool>> SendNotificationToAdminsAsync(string message, UserNotificationType type, CancellationToken cancellationToken = default);
}

