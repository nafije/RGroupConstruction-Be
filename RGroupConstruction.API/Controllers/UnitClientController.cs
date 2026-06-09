using RGroupConstruction.API.Controllers.Base;
using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.UnitClient.Commands.CreateUnitClient;
using RGroupConstruction.Application.Features.UnitClient.Commands.DeleteUnitClient;
using RGroupConstruction.Application.Features.UnitClient.Commands.UpdateUnitClient;
using RGroupConstruction.Application.Features.UnitClient.Queries.GetAllPagedUnitClients;
using RGroupConstruction.Application.Features.UnitClient.Queries.GetDashboardStats;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RGroupConstruction.API.Controllers;

[Authorize(SystemPolicies.Admin)]
public class UnitClientController(IMediator _mediator, IMessageLocalizer localizer) : ApiControllerBase(_mediator, localizer)
{
    [HttpPost("create")]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUnitClient([FromBody] CreateUnitClientCommand command)
    {
        return await SendCommand(command, _localizer[MessageKeys.Success.UnitClient.Created]);
    }

    [HttpPut("update/{id}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUnitClient([FromRoute] string id, [FromBody] UpdateUnitClientCommand command)
    {
        var updatedCommand = command with { Id = id };
        return await SendCommand(updatedCommand, _localizer[MessageKeys.Success.UnitClient.Updated]);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUnitClient([FromRoute] string id)
    {
        return await SendCommand(new DeleteUnitClientCommand(id), _localizer[MessageKeys.Success.UnitClient.Deleted]);
    }

    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedResponse<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllPagedUnitClients([FromQuery] GetAllPagedUnitClientsQuery query)
    {
        return await SendQuery(query, null, StatusCodes.Status404NotFound);
    }

    [HttpGet("stats")]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDashboardStats()
    {
        return await SendQuery(new GetDashboardStatsQuery(), null, StatusCodes.Status404NotFound);
    }
}

