using Bebrakumpis.Application.Common.Result;
using Microsoft.AspNetCore.Mvc;

namespace Bebrakumpis.API.Common;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller, Func<T, IActionResult> onSuccess)
        => result.IsSuccess ? onSuccess(result.Value) : result.ToProblemResult(controller);

    public static IActionResult ToActionResult(this Result result, ControllerBase controller, Func<IActionResult> onSuccess)
        => result.IsSuccess ? onSuccess() : result.ToProblemResult(controller);

    public static IActionResult ToProblemResult<T>(this Result<T> result, ControllerBase controller)
        => ToProblemDetails(result.ErrorType, result.Error, result.Errors, controller);

    public static IActionResult ToProblemResult(this Result result, ControllerBase controller)
        => ToProblemDetails(result.ErrorType, result.Error, result.Errors, controller);

    private static IActionResult ToProblemDetails(
        ErrorType errorType,
        string? error,
        IEnumerable<string> errors,
        ControllerBase controller)
    {
        return errorType switch
        {
            ErrorType.NotFound => controller.NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Resource not found.",
                Detail = error
            }),
            ErrorType.ValidationFailure => controller.BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation failed.",
                Detail = string.Join("; ", errors),
                Extensions = { ["errors"] = errors }
            }),
            ErrorType.Conflict => controller.Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict.",
                Detail = error
            }),
            ErrorType.Unauthorized => controller.Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized.",
                Detail = error
            }),
            _ => controller.BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Detail = error
            })
        };
    }
}
