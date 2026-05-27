namespace Bebrakumpis.Application.Common.Result;

public static class ResultExtensions
{
    public static TOut Match<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, TOut> onSuccess,
        Func<Result<TIn>, TOut> onFailure)
    {
        return result.IsSuccess ? onSuccess(result.Value!) : onFailure(result);
    }

    public static TOut Match<TOut>(
        this Result result,
        Func<TOut> onSuccess,
        Func<Result, TOut> onFailure)
    {
        return result.IsSuccess ? onSuccess() : onFailure(result);
    }

    public static Result<TOut> PropagateFailure<TIn, TOut>(this Result<TIn> result)
    {
        return result.ErrorType switch
        {
            ErrorType.NotFound => Result<TOut>.NotFound(result.Error!),
            ErrorType.Conflict => Result<TOut>.Conflict(result.Error!),
            ErrorType.Unauthorized => Result<TOut>.Unauthorized(result.Error!),
            _ => Result<TOut>.ValidationFailure(result.Errors)
        };
    }

    public static Result<TOut> PropagateFailure<TOut>(this Result result)
    {
        return result.ErrorType switch
        {
            ErrorType.NotFound => Result<TOut>.NotFound(result.Error!),
            ErrorType.Conflict => Result<TOut>.Conflict(result.Error!),
            ErrorType.Unauthorized => Result<TOut>.Unauthorized(result.Error!),
            _ => Result<TOut>.ValidationFailure(result.Errors)
        };
    }
}
