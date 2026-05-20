using HotelBookingPlatform.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingPlatform.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result, ControllerBase controller)
    {
        if (result.IsSuccess)
            return controller.NoContent();

        return result.ErrorCode switch
        {
            "NOT_FOUND" => controller.NotFound(new ProblemDetail(result.ErrorMessage!, result.ErrorCode!)),
            "VALIDATION_ERROR" => controller.BadRequest(new ProblemDetail(result.ErrorMessage!, result.ErrorCode!)),
            "INVENTORY_CONFLICT" => controller.Conflict(new ProblemDetail(result.ErrorMessage!, result.ErrorCode!)),
            "BOOKING_INVALID_STATE" => controller.UnprocessableEntity(new ProblemDetail(result.ErrorMessage!, result.ErrorCode!)),
            "BOOKING_ALREADY_CANCELLED" => controller.UnprocessableEntity(new ProblemDetail(result.ErrorMessage!, result.ErrorCode!)),
            "CAPACITY_EXCEEDED" => controller.UnprocessableEntity(new ProblemDetail(result.ErrorMessage!, result.ErrorCode!)),
            _ => controller.BadRequest(new ProblemDetail(result.ErrorMessage!, result.ErrorCode ?? "ERROR"))
        };
    }

    public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller, bool created = false, string? routeName = null, object? routeValues = null)
    {
        if (result.IsSuccess)
        {
            if (created && routeName is not null)
                return controller.CreatedAtRoute(routeName, routeValues, result.Value);
            if (created)
                return controller.StatusCode(StatusCodes.Status201Created, result.Value);
            return controller.Ok(result.Value);
        }

        return ((Result)result).ToActionResult(controller);
    }
}

public sealed record ProblemDetail(string Message, string ErrorCode);
