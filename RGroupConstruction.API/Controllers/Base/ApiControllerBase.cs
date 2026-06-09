using RGroupConstruction.API.Exctention;
using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs.ApiResponse;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace RGroupConstruction.API.Controllers.Base;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase(IMediator mediator, IMessageLocalizer localizer) : ControllerBase
{
    protected readonly IMediator _mediator = mediator;
    protected readonly IMessageLocalizer _localizer = localizer;

    protected (string? UserId, IActionResult? UnauthorizedResult) GetCurrentUserIdOrUnauthorized()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return (null, Unauthorized(ApiResponse.FailureResponse(_localizer[MessageKeys.Api.UserNotAuthenticated])));
        return (userId, null);
    }

    protected async Task<IActionResult> SendQuery<TResponse>(IRequest<Result<TResponse>> query)
    {
        var result = await _mediator.Send(query);
        return result.ToActionResult(localizer: _localizer);
    }

    protected async Task<IActionResult> SendQuery<TResponse>(
       IRequest<Result<TResponse>> query,
       string? successMessage = null,
       int failureStatusCode = StatusCodes.Status400BadRequest)
    {
        var result = await _mediator.Send(query);
        if (!result.IsSuccessful)
            return StatusCode(
                failureStatusCode,
                ApiResponse<TResponse>.FailureResponse(LocalizeError(result.ErrorResult)));

        return Ok(ApiResponse<TResponse>.SuccessResponse(result.Data!, successMessage));
    }

    protected async Task<IActionResult> SendCommand<TResponse>(IRequest<Result<TResponse>> command)
    {
        var result = await _mediator.Send(command);
        return result.ToActionResult(localizer: _localizer);
    }

    protected async Task<IActionResult> SendCommand<TResponse>(
        IRequest<Result<TResponse>> command,
        string? successMessage,
        int failureStatusCode = StatusCodes.Status400BadRequest)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccessful)
            return StatusCode(
                failureStatusCode,
                ApiResponse<TResponse>.FailureResponse(LocalizeError(result.ErrorResult)));

        return Ok(ApiResponse<TResponse>.SuccessResponse(result.Data!, successMessage ?? _localizer[MessageKeys.Success.General.OperationCompleted]));
    }

    protected async Task<IActionResult> SendCommand(
       IRequest<Result> command,
       string? successMessage = null,
       int failureStatusCode = StatusCodes.Status400BadRequest)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccessful)
            return StatusCode(
                failureStatusCode,
                ApiResponse.FailureResponse(LocalizeError(result.ErrorResult)));

        return Ok(ApiResponse.SuccessResponse(successMessage ?? _localizer[MessageKeys.Success.General.OperationCompleted]));
    }

    protected async Task<IActionResult> SendCommandCreated<TResponse>(
        IRequest<Result<TResponse>> command,
        string routeName,
        object routeValues,
        string? successMessage = null)
    {
        var result = await _mediator.Send(command);
        return result.ToCreatedResult(routeName, routeValues, successMessage, _localizer);
    }

    protected async Task<IActionResult> SendCommandNoContent(IRequest<Result> command)
    {
        var result = await _mediator.Send(command);
        return result.ToNoContentResult(_localizer);
    }

    protected async Task<IActionResult> SendPaginatedQuery<TDto>(
        IRequest<Result<List<TDto>>> query,
        int totalCount,
        int pageNumber,
        int pageSize)
    {
        var result = await _mediator.Send(query);

        if (!result.IsSuccessful)
            return result.ToActionResult(localizer: _localizer);

        return result.Data!.ToPaginatedResult(totalCount, pageNumber, pageSize);
    }

    private string LocalizeError(string? errorResult)
        => string.IsNullOrWhiteSpace(errorResult)
            ? _localizer[MessageKeys.Api.InvalidRequest]
            : _localizer[errorResult];
}

