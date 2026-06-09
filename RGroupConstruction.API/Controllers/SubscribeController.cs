using RGroupConstruction.API.Controllers.Base;
using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Subscribe.Commands.CreateSubscription;
using RGroupConstruction.Application.Features.Subscribe.Commands.RemoveSubscription;
using RGroupConstruction.Application.Features.Subscribe.Queries.GetAllPagedSubscriptions;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RGroupConstruction.API.Controllers;

public class SubscribeController(IMediator _mediator, IMessageLocalizer localizer) : ApiControllerBase(_mediator, localizer)
{
    [HttpPost("subscribe")]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Subscribe([FromBody] CreateSubscriptionCommand command)
    {
        return await SendCommand(command, _localizer[MessageKeys.Success.Subscribe.Created]);
    }

    [HttpPost("unsubscribe")]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Unsubscribe([FromBody] RemoveSubscriptionCommand command)
    {
        return await SendCommand(command, _localizer[MessageKeys.Success.Subscribe.Removed]);
    }


    [HttpGet("getPagedSubscription", Name = nameof(GetAllPagedSubscriptions))]
    [Authorize(SystemPolicies.Admin)]
    [ProducesResponseType(typeof(Result<PagedResponse<SubscribeDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllPagedSubscriptions([FromQuery] GetAllPagedSubscriptionsQuery query)
    {
        return await SendQuery(query, null, StatusCodes.Status404NotFound);
    }
}

