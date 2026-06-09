using RGroupConstruction.API.Controllers.Base;
using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Department.Commands.CreateDepartment;
using RGroupConstruction.Application.Features.Department.Commands.DeleteDepartment;
using RGroupConstruction.Application.Features.Department.Commands.UpdateDepartment;
using RGroupConstruction.Application.Features.Department.Queries.GetAllDepartments;
using RGroupConstruction.Application.Features.Department.Queries.GetAllPagedDepartments;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RGroupConstruction.API.Controllers;

public class DepartmentController(IMediator _mediator, IMessageLocalizer localizer) : ApiControllerBase(_mediator, localizer)
{
    [HttpPost("create")]
    [Authorize(SystemPolicies.Admin)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentCommand command)
    {
        return await SendCommand(command, _localizer[MessageKeys.Success.Department.Created]);
    }

    [HttpPut("update/{id}")]
    [Authorize(SystemPolicies.Admin)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDepartment([FromRoute] string id, [FromBody] UpdateDepartmentCommand command)
    {
        var updatedCommand = command with { Id = id };
        return await SendCommand(updatedCommand, _localizer[MessageKeys.Success.Department.Updated]);
    }

    [HttpDelete("{id}")]
    [Authorize(SystemPolicies.Admin)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDepartment([FromRoute] string id)
    {
        return await SendCommand(new DeleteDepartmentCommand(id), _localizer[MessageKeys.Success.Department.Deleted]);
    }

    [HttpGet]
    [Authorize(SystemPolicies.Admin)]
    [ProducesResponseType(typeof(Result<PagedResponse<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllPagedDepartments([FromQuery] GetAllPagedDepartmentsQuery query)
    {
        return await SendQuery(query, null, StatusCodes.Status404NotFound);
    }

    [HttpGet("getAll")]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllDepartments([FromQuery] GetAllDepartmentsQuery query)
    {
        return await SendQuery(query, null, StatusCodes.Status404NotFound);
    }

}


