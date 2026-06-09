using RGroupConstruction.API.Controllers.Base;
using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Ads.Commands.CreateAds;
using RGroupConstruction.Application.Features.Ads.Commands.DeleteAds;
using RGroupConstruction.Application.Features.Ads.Commands.UpdateAds;
using RGroupConstruction.Application.Features.Ads.Queries.GetAllAds;
using RGroupConstruction.Application.Features.Ads.Queries.GetAllPagedAds;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RGroupConstruction.API.Controllers;


public class AdsController(IMediator _mediator, IMessageLocalizer localizer) : ApiControllerBase(_mediator, localizer)
{
    [HttpPost(Name = nameof(CreateAds))]
    [Authorize(SystemPolicies.Admin)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAds([FromBody] CreateAdsCommand command)
    {
        return await SendCommand(command, _localizer[MessageKeys.Success.Ads.Created]);
    }

    [HttpPut("{id}", Name = nameof(UpdateAds))]
    [Authorize(SystemPolicies.Admin)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAds(
       [FromRoute] string id,
       [FromBody] UpdateAdsCommand command)
    {
        var updatedCommand = command with { Id = id };
        return await SendCommand(updatedCommand, _localizer[MessageKeys.Success.Ads.Updated]);
    }

    [HttpDelete("{id}", Name = nameof(DeleteAds))]
    [Authorize(SystemPolicies.Admin)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteAds([FromRoute] string id)
    {
        var command = new DeleteAdsCommand { Id = id };
        return await SendCommand(command, _localizer[MessageKeys.Success.Ads.Deleted]);
    }

    [HttpGet("getAll", Name = nameof(GetAllAds))]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllAds()
    {
        return await SendQuery(new GetAllAdsQuery(), null, StatusCodes.Status404NotFound);
    }

    [HttpGet("getPagedAds", Name = nameof(GetAllPagedAds))]
    [Authorize(SystemPolicies.Admin)]
    [ProducesResponseType(typeof(Result<PagedResponse<AdsDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllPagedAds([FromQuery] GetAllPagedAdsQuery query)
    {
        return await SendQuery(query, null, StatusCodes.Status404NotFound);
    }
}


