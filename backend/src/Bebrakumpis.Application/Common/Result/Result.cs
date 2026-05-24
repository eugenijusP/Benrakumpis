using System.Diagnostics.CodeAnalysis;

namespace Bebrakumpis.Application.Common.Result;

public class Result<T>
{
    public T? Value { get; }

    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsSuccess { get; }
    public ErrorType ErrorType { get; }
    public IReadOnlyList<string> Errors { get; }
    public string? Error { get; }

    private Result(T value)
    {
        Value = value;
        IsSuccess = true;
        ErrorType = ErrorType.None;
        Errors = [];
    }

    private Result(ErrorType errorType, string message, IEnumerable<string>? errors = null)
    {
        IsSuccess = false;
        ErrorType = errorType;
        Error = message;
        Errors = errors?.ToList() ?? [message];
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> NotFound(string message) => new(ErrorType.NotFound, message);
    public static Result<T> ValidationFailure(string message) => new(ErrorType.ValidationFailure, message);
    public static Result<T> ValidationFailure(IEnumerable<string> errors)
    {
        var list = errors.ToList();
        return new(ErrorType.ValidationFailure, list.FirstOrDefault() ?? "Validation failed.", list);
    }
    public static Result<T> Conflict(string message) => new(ErrorType.Conflict, message);
    public static Result<T> Unauthorized(string message) => new(ErrorType.Unauthorized, message);
}

public class Result
{
    public bool IsSuccess { get; }
    public ErrorType ErrorType { get; }
    public IReadOnlyList<string> Errors { get; }
    public string? Error { get; }

    private Result()
    {
        IsSuccess = true;
        ErrorType = ErrorType.None;
        Errors = [];
    }

    private Result(ErrorType errorType, string message, IEnumerable<string>? errors = null)
    {
        IsSuccess = false;
        ErrorType = errorType;
        Error = message;
        Errors = errors?.ToList() ?? [message];
    }

    public static Result Success() => new();
    public static Result NotFound(string message) => new(ErrorType.NotFound, message);
    public static Result ValidationFailure(string message) => new(ErrorType.ValidationFailure, message);
    public static Result ValidationFailure(IEnumerable<string> errors)
    {
        var list = errors.ToList();
        return new(ErrorType.ValidationFailure, list.FirstOrDefault() ?? "Validation failed.", list);
    }
    public static Result Conflict(string message) => new(ErrorType.Conflict, message);
    public static Result Unauthorized(string message) => new(ErrorType.Unauthorized, message);
}
