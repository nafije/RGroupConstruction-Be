using RGroupConstruction.API.Controllers.Base;
using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Features.Parking.Commands.CreatePrakingUnit;
using RGroupConstruction.Application.Features.Parking.Commands.DeleteParkingUnit;
using RGroupConstruction.Application.Features.Parking.Commands.UpdatePrakingUnit;
using RGroupConstruction.Application.Features.Parking.Queries.GetAllParkingUnits;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RGroupConstruction.API.Controllers;

public class ParkingController(IMediator _mediator, IMessageLocalizer localizer) : ApiControllerBase(_mediator, localizer)
{

    [HttpPost("create")]
    [Authorize(SystemPolicies.Admin)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePrakingUnit([FromBody] CreatePrakingCommand command)
    {
        return await SendCommand(command, _localizer[MessageKeys.Success.Parking.Created]);
    }

    [HttpPut("update{id}")]
    [Authorize(SystemPolicies.Admin)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePrakingUnit([FromBody] UpdatePrakingCommand command, [FromRoute] string id)
    {
        command.Id = id;
        return await SendCommand(command, _localizer[MessageKeys.Success.Parking.Updated]);
    }

    [HttpDelete("{id}")]
    [Authorize(SystemPolicies.Admin)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteParkingUnit([FromRoute] string id)
    {
        return await SendCommand(new DeleteParkingCommand(id), _localizer[MessageKeys.Success.Parking.Created]);
    }
    [HttpGet]
    [ProducesResponseType(typeof(Result<Result<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllParkingUnits([FromQuery] GetAllParkingsQuery query)
    {
        return await SendQuery(query, null, StatusCodes.Status404NotFound);
    }
}

