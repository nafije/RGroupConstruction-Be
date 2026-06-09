using RGroupConstruction.API.Controllers.Base;
using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Project.Queries.GetAllProjects;
using RGroupConstruction.Application.Features.Unit.Commands.CreateUnit;
using RGroupConstruction.Application.Features.Unit.Commands.DeleteUnit;
using RGroupConstruction.Application.Features.Unit.Commands.UpdateUnit;
using RGroupConstruction.Application.Features.Unit.Queries.GetAllPagedUnits;
using RGroupConstruction.Application.Features.Unit.Queries.GetAllUnits;
using RGroupConstruction.Application.Features.Unit.Queries.GetSimilarlUnits;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RGroupConstruction.API.Controllers;

public class UnitController(IMediator _mediator, IMessageLocalizer localizer) : ApiControllerBase(_mediator, localizer)
{
    [HttpPost("create")]
    [Authorize(SystemPolicies.Admin)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUnit([FromBody] CreateUnitCommand command)
    {
        return await SendCommand(command, _localizer[MessageKeys.Success.Unit.Created]);
    }

    [HttpPut("update/{id}")]
    [Authorize(SystemPolicies.Admin)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUnit([FromRoute] string id, [FromBody] UpdateUnitCommand command)
    {
        var updatedCommand = command with { Id = id };
        return await SendCommand(updatedCommand, _localizer[MessageKeys.Success.Unit.Created]);
    }

    [HttpDelete("{id}")]
    [Authorize(SystemPolicies.Admin)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUnit([FromRoute] string id)
    {
        return await SendCommand(new DeleteUnitCommand(id), _localizer[MessageKeys.Success.Unit.Created]);
    }

    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedResponse<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllPagedUnits([FromQuery] GetAllPagedUnitsQuery query)
    {
        return await SendQuery(query, null, StatusCodes.Status404NotFound);
    }


    [HttpGet("getAll")]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllUnits([FromQuery] GetAllUnitsQuery query)
    {
        return await SendQuery(query, null, StatusCodes.Status404NotFound);
    }

    [HttpGet("similarUnits/{id}")]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSimilarUnits([FromRoute] string id)
    {
        return await SendQuery(new GetSimilarlUnitsQuery(id), null, StatusCodes.Status404NotFound);
    }
}

