namespace Bebrakumpis.Application.Common.Result;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public ErrorType ErrorType { get; }

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
        ErrorType = ErrorType.None;
    }

    private Result(ErrorType errorType, string error)
    {
        IsSuccess = false;
        ErrorType = errorType;
        Error = error;
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> NotFound(string error) => new(ErrorType.NotFound, error);
    public static Result<T> ValidationFailure(string error) => new(ErrorType.ValidationFailure, error);
    public static Result<T> Conflict(string error) => new(ErrorType.Conflict, error);
    public static Result<T> Unauthorized(string error) => new(ErrorType.Unauthorized, error);
}
