using Bebrakumpis.Application.Common.Result;
using Microsoft.AspNetCore.Mvc;

namespace Bebrakumpis.API.Common;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
    {
        if (result.IsSuccess)
            return controller.Ok(result.Value);

        return result.ErrorType switch
        {
            ErrorType.NotFound => controller.NotFound(new ProblemDetails { Detail = result.Error }),
            ErrorType.ValidationFailure => controller.BadRequest(new ProblemDetails { Detail = result.Error }),
            ErrorType.Conflict => controller.Conflict(new ProblemDetails { Detail = result.Error }),
            ErrorType.Unauthorized => controller.Unauthorized(new ProblemDetails { Detail = result.Error }),
            _ => controller.StatusCode(500, new ProblemDetails { Detail = "An unexpected error occurred." })
        };
    }
}
