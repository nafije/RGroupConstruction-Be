using RGroupConstruction.API.Controllers.Base;
using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Log.Queries.GetAllPagedLogs;
using RGroupConstruction.Application.Features.Log.Queries.GetLogById;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Domain.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RGroupConstruction.API.Controllers;

public class LogsController(IMediator mediator, IMessageLocalizer localizer) : ApiControllerBase(mediator, localizer)
{
    [HttpGet]
    [Authorize(SystemPolicies.Admin)]
    [ProducesResponseType(typeof(Result<PagedResponse<LogEntryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllPagedLogs([FromQuery] GetAllPagedLogsQuery query)
    {
        return await SendQuery(query, null, StatusCodes.Status404NotFound);
    }

    [HttpGet("{id:int}")]
    [Authorize(SystemPolicies.Admin)]
    [ProducesResponseType(typeof(Result<LogEntryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLogById([FromRoute] int id)
    {
        return await SendQuery(new GetLogByIdQuery(id), null, StatusCodes.Status404NotFound);
    }
}

