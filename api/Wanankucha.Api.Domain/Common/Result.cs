namespace Wanankucha.Api.Domain.Common;

/// <summary>
/// Represents the result of an operation that can either succeed or fail.
/// </summary>
/// <typeparam name="T">The type of the value on success</typeparam>
public record Result<T>
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public Error? Error { get; init; }
    public IReadOnlyList<Error> Errors { get; init; } = [];

    private Result(bool isSuccess, T? value, Error? error, IReadOnlyList<Error>? errors = null)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        Errors = errors ?? [];
    }

    public static Result<T> Success(T value) => new(true, value, null);

    public static Result<T> Failure(Error error) => new(false, default, error);

    public static Result<T> Failure(IEnumerable<Error> errors)
    {
        var errorList = errors.ToList();
        return new(false, default, errorList.FirstOrDefault() ?? Common.Error.None, errorList);
    }
}

/// <summary>
/// Represents the result of an operation without a return value.
/// </summary>
public record Result
{
    public bool IsSuccess { get; init; }
    public Error? Error { get; init; }
    public IReadOnlyList<Error> Errors { get; init; } = [];

    private Result(bool isSuccess, Error? error, IReadOnlyList<Error>? errors = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        Errors = errors ?? [];
    }

    public static Result Success() => new(true, null);

    public static Result Failure(Error error) => new(false, error);

    public static Result Failure(IEnumerable<Error> errors)
    {
        var errorList = errors.ToList();
        return new(false, errorList.FirstOrDefault() ?? Common.Error.None, errorList);
    }
}

