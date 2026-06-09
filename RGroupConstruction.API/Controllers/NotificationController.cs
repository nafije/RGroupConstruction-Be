using RGroupConstruction.API.Controllers.Base;
using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Notifications.Commands.AdminDeleteNotification;
using RGroupConstruction.Application.Features.Notifications.Commands.AdminMarkNotificationRead;
using RGroupConstruction.Application.Features.Notifications.Commands.MarkAllNotificationsRead;
using RGroupConstruction.Application.Features.Notifications.Queries.GetAdminNotifications;
using RGroupConstruction.Application.Features.Notifications.Queries.GetUnreadCount;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RGroupConstruction.API.Controllers;


[Authorize(SystemPolicies.Admin)]
public class NotificationController(IMediator _mediator, IMessageLocalizer localizer) : ApiControllerBase(_mediator, localizer)
{

    [HttpGet("admin", Name = nameof(GetAllNotificationsAdmin))]
    [ProducesResponseType(typeof(Result<PagedResponse<UserNotificationDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllNotificationsAdmin(
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var (userId, unauthorizedResult) = GetCurrentUserIdOrUnauthorized();

        if (unauthorizedResult is not null)
            return unauthorizedResult;

        return await SendQuery(new GetAdminNotificationsQuery
        {
            UserId = Guid.Parse(userId!),
            PageNumber = pageNumber,
            PageSize = pageSize
        });
    }

    [HttpGet("unread-count", Name = nameof(GetUnreadCount))]
    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUnreadCount()
    {
        var (userId, unauthorizedResult) = GetCurrentUserIdOrUnauthorized();
        if (unauthorizedResult is not null) return unauthorizedResult;

        return await SendQuery(new GetUnreadCountQuery { UserId = Guid.Parse(userId!) });
    }

    [HttpPatch("admin/{id}/read", Name = nameof(AdminMarkNotificationRead))]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AdminMarkNotificationRead([FromRoute] Guid id)
    {
        return await SendCommand(
            new AdminMarkNotificationReadCommand { NotificationId = id },
            _localizer[MessageKeys.Success.Notification.MarkedRead]);
    }

    [HttpPatch("read-all", Name = nameof(MarkAllNotificationsRead))]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MarkAllNotificationsRead()
    {
        var (userId, unauthorizedResult) = GetCurrentUserIdOrUnauthorized();
        if (unauthorizedResult is not null) return unauthorizedResult;

        return await SendCommand(
            new MarkAllNotificationsReadCommand { UserId = Guid.Parse(userId!) },
            _localizer[MessageKeys.Success.Notification.AllMarkedRead]);
    }

    [HttpDelete("admin/{id}", Name = nameof(AdminDeleteNotification))]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AdminDeleteNotification([FromRoute] Guid id)
    {
        return await SendCommand(
            new AdminDeleteNotificationCommand { NotificationId = id },
            _localizer[MessageKeys.Success.Notification.Deleted]);
    }
}

