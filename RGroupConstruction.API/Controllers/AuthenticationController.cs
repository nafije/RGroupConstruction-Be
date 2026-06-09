using RGroupConstruction.API.Controllers.Base;
using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs.ApiResponse;
using RGroupConstruction.Application.Features.Authentication.Command.Login;
using RGroupConstruction.Application.Features.Authentication.Command.Logout;
using RGroupConstruction.Application.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RGroupConstruction.API.Controllers;

public class AuthenticationController(IMediator _mediator, IMessageLocalizer localizer) : ApiControllerBase(_mediator, localizer)
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        return await SendCommand(command);
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        var (userId, unauthorizedResult) = GetCurrentUserIdOrUnauthorized();
        if (unauthorizedResult is not null)
            return unauthorizedResult;

        var command = new LogoutCommand(userId!);
        return await SendCommand(command);
    }

}

