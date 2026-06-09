using RGroupConstruction.API.Controllers.Base;
using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Status.Commands.CreateStatus;
using RGroupConstruction.Application.Features.Status.Commands.DeleteStatus;
using RGroupConstruction.Application.Features.Status.Commands.UpdateStatus;
using RGroupConstruction.Application.Features.Status.Queries.GetAllPagedStatuses;
using RGroupConstruction.Application.Features.Status.Queries.GetAllStatuses;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RGroupConstruction.API.Controllers;

public class StatusController(IMediator _mediator, IMessageLocalizer localizer) : ApiControllerBase(_mediator, localizer)
{
    [HttpPost("create")]
    [Authorize(SystemPolicies.Admin)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateStatus([FromBody] CreateStatusCommand command)
    {
        return await SendCommand(command, _localizer[MessageKeys.Success.Status.Created]);
    }

    [HttpPut("update/{id}")]
    [Authorize(SystemPolicies.Admin)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus([FromRoute] string id, [FromBody] UpdateStatusCommand command)
    {
        var updatedCommand = command with { Id = id };
        return await SendCommand(updatedCommand, _localizer[MessageKeys.Success.Status.Updated]);
    }

    [HttpDelete("{id}")]
    [Authorize(SystemPolicies.Admin)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStatus([FromRoute] string id)
    {
        return await SendCommand(new DeleteStatusCommand(id), _localizer[MessageKeys.Success.Status.Deleted]);
    }

    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedResponse<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllPagedStatuses([FromQuery] GetAllPagedStatusesQuery query)
    {
        return await SendQuery(query, null, StatusCodes.Status404NotFound);
    }

    [HttpGet("getAll")]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllStatuses([FromQuery] GetAllStatusesQuery query)
    {
        return await SendQuery(query, null, StatusCodes.Status404NotFound);
    }
}


