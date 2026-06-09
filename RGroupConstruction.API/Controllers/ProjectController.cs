using RGroupConstruction.API.Controllers.Base;
using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Project.Commands.AddFeaturedProject;
using RGroupConstruction.Application.Features.Project.Commands.CreateProject;
using RGroupConstruction.Application.Features.Project.Commands.DeleteProject;
using RGroupConstruction.Application.Features.Project.Commands.UpdateProject;
using RGroupConstruction.Application.Features.Project.Queries.GetAllPagedProjects;
using RGroupConstruction.Application.Features.Project.Queries.GetAllProjects;
using RGroupConstruction.Application.Features.Project.Queries.GetFeaturedProjects;
using RGroupConstruction.Application.Features.Project.Queries.GetProjectById;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace RGroupConstruction.API.Controllers;

public class ProjectController(IMediator _mediator, IMessageLocalizer localizer) : ApiControllerBase(_mediator, localizer)
{
    [HttpPost("create")]
    [Authorize(SystemPolicies.Admin)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectCommand command)
    {
        return await SendCommand(command, _localizer[MessageKeys.Success.Project.Created]);
    }

    [HttpPut("update/{id}")]
    [Authorize(SystemPolicies.Admin)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProject([FromRoute] string id, [FromBody] UpdateProjectCommand command)
    {
        var updatedCommand = command with { Id = id };
        return await SendCommand(updatedCommand, _localizer[MessageKeys.Success.Project.Updated]);
    }

    [HttpDelete("{id}")]
    [Authorize(SystemPolicies.Admin)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProject([FromRoute] string id)
    {
        return await SendCommand(new DeleteProjectCommand(id), _localizer[MessageKeys.Success.Project.Deleted]);
    }


    [HttpGet("featured")]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFeaturedProjects([FromQuery] GetFeaturedProjectsQuery query)
    {
        return await SendQuery(query, null, StatusCodes.Status404NotFound);
    }

    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedResponse<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllPagedProjects([FromQuery] GetAllPagedProjectsQuery query)
    {
        return await SendQuery(query, null, StatusCodes.Status404NotFound);
    }
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<PagedResponse<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProjectById([FromRoute] string id)
    {
        var query = new GetProjectByIdQuery()
        {
            ProjectId = id
        };
        return await SendQuery(query, null, StatusCodes.Status404NotFound);
    }


    [HttpGet("getAll")]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllProjects([FromQuery] GetAllProjectsQuery query)
    {
        return await SendQuery(query, null, StatusCodes.Status404NotFound);
    }


    [HttpPost("add-featured-project")]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddFeaturedProject([FromBody] AddFeaturedProjectCommand command)
    {
        return await SendCommand(command, _localizer[MessageKeys.Success.Project.FeaturedUpdated]);
    }
}

