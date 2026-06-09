using RGroupConstruction.API.Controllers.Base;
using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Job.Commands.CreateJob;
using RGroupConstruction.Application.Features.Job.Commands.DeleteJob;
using RGroupConstruction.Application.Features.Job.Commands.UpdateJob;
using RGroupConstruction.Application.Features.Job.Commands.UpdateJobActivityStatus;
using RGroupConstruction.Application.Features.Job.Queries.GetAllActiveJobs;
using RGroupConstruction.Application.Features.Job.Queries.GetAllJobs;
using RGroupConstruction.Application.Features.Job.Queries.GetAllPagedJobs;
using RGroupConstruction.Application.Features.Job.Queries.GetJobById;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RGroupConstruction.API.Controllers;

public class JobController(IMediator _mediator, IMessageLocalizer localizer) : ApiControllerBase(_mediator, localizer)
{
    [HttpPost("create")]
    [Authorize(SystemPolicies.Admin)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateJob([FromBody] CreateJobCommand command)
    {
        return await SendCommand(command, _localizer[MessageKeys.Success.Job.Created]);
    }

    [HttpPut("update/{id}")]
    [Authorize(SystemPolicies.Admin)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateJob([FromRoute] string id, [FromBody] UpdateJobCommand command)
    {
        var updatedCommand = command with { Id = id };
        return await SendCommand(updatedCommand, _localizer[MessageKeys.Success.Job.Updated]);
    }

    [HttpDelete("{id}")]
    [Authorize(SystemPolicies.Admin)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteJob([FromRoute] string id)
    {
        return await SendCommand(new DeleteJobCommand(id), _localizer[MessageKeys.Success.Job.Deleted]);
    }


    [HttpGet]
    [Authorize(SystemPolicies.Admin)]
    [ProducesResponseType(typeof(Result<PagedResponse<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllPagedJobs([FromQuery] GetAllPagedJobsQuery query)
    {
        return await SendQuery(query, null, StatusCodes.Status404NotFound);
    }

    [HttpGet("getActiveJobs")]
    [ProducesResponseType(typeof(Result<PagedResponse<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllActiveJobs([FromQuery] GetAllActiveJobsQuery query)
    {
        return await SendQuery(query, null, StatusCodes.Status404NotFound);
    }


    [HttpGet("getAll")]
    [Authorize(SystemPolicies.Admin)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllJobs([FromQuery] GetAllJobsQuery query)
    {
        return await SendQuery(query, null, StatusCodes.Status404NotFound);
    }


    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetJobById([FromRoute] string id)
    {
        return await SendQuery(new GetJobByIdQuery(id) , null, StatusCodes.Status404NotFound);
    }


    [HttpPost("activity")]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateJobActivityStatus([FromBody] UpdateJobActivityStatusCommand command)
    {
        return await SendCommand(command, _localizer[MessageKeys.Success.Job.CareerStatusUpdated]);
    }
}

