using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.DTOs.ApiResponse;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using Microsoft.AspNetCore.Mvc;

namespace RGroupConstruction.API.Exctention;

public static class ControllerExtensions
{
    public static IActionResult ToActionResult(
        this Result result,
        IMessageLocalizer? localizer = null)
    {
        if (result.IsSuccessful)
        {
            var message = Localize(MessageKeys.Success.General.OperationCompleted, localizer);

            return new OkObjectResult(ApiResponse.SuccessResponse(message));
        }

        return new BadRequestObjectResult(
            ApiResponse.FailureResponse(Localize(result.ErrorResult!, localizer)));
    }

    public static IActionResult ToActionResult<T>(
        this Result<T> result,
        string? successMessage = null,
        IMessageLocalizer? localizer = null)
    {
        if (result.IsSuccessful)
        {
            var message = successMessage ??
                          Localize(MessageKeys.Success.General.OperationCompleted, localizer);

            return new OkObjectResult(
                ApiResponse<T>.SuccessResponse(result.Data!, message));
        }

        return new BadRequestObjectResult(
            ApiResponse<T>.FailureResponse(Localize(result.ErrorResult!, localizer)));
    }

    public static IActionResult ToCreatedResult<T>(
        this Result<T> result,
        string routeName,
        object routeValues,
        string? successMessage = null,
        IMessageLocalizer? localizer = null)
    {
        if (result.IsSuccessful)
        {
            var message =
                successMessage ??
                Localize(MessageKeys.Success.General.ResourceCreated, localizer);

            return new CreatedAtRouteResult(
                routeName,
                routeValues,
                ApiResponse<T>.SuccessResponse(result.Data!, message));
        }

        return new BadRequestObjectResult(
            ApiResponse<T>.FailureResponse(Localize(result.ErrorResult!, localizer)));
    }

    public static IActionResult ToNoContentResult(
        this Result result,
        IMessageLocalizer? localizer = null)
    {
        if (result.IsSuccessful)
            return new NoContentResult();

        return new BadRequestObjectResult(
            ApiResponse.FailureResponse(Localize(result.ErrorResult!, localizer)));
    }

    public static IActionResult ToPaginatedResult<T>(
        this IEnumerable<T> items,
        int totalCount,
        int pageNumber,
        int pageSize,
        IMessageLocalizer? localizer = null)
    {
        var pagedResponse = new PagedResponse<T>(
            items.ToList(),
            totalCount,
            pageNumber,
            pageSize);

        var message =
            localizer?[MessageKeys.Success.General.OperationCompleted];

        return new OkObjectResult(
            ApiResponse<PagedResponse<T>>.SuccessResponse(pagedResponse, message));
    }

    private static string Localize(string errorKey, IMessageLocalizer? localizer)
        => localizer is null ? errorKey : localizer[errorKey];
}
