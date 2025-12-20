namespace Wanankucha.Api.Domain.Common;

/// <summary>
/// Represents the result of an operation that can either succeed or fail.
/// </summary>
/// <typeparam name="T">The type of the value on success</typeparam>
public record Result<T>
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public string? Error { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = [];

    private Result(bool isSuccess, T? value, string? error, IReadOnlyList<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        Errors = errors ?? [];
    }

    public static Result<T> Success(T value) => new(true, value, null);

    public static Result<T> Failure(string error) => new(false, default, error);

    public static Result<T> Failure(IEnumerable<string> errors)
    {
        var errorList = errors.ToList();
        return new(false, default, errorList.FirstOrDefault(), errorList);
    }

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<string, TResult> onFailure)
        => IsSuccess ? onSuccess(Value!) : onFailure(Error ?? "Unknown error");
}

/// <summary>
/// Represents the result of an operation without a return value.
/// </summary>
public record Result
{
    public bool IsSuccess { get; init; }
    public string? Error { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = [];

    private Result(bool isSuccess, string? error, IReadOnlyList<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        Errors = errors ?? [];
    }

    public static Result Success() => new(true, null);

    public static Result Failure(string error) => new(false, error);

    public static Result Failure(IEnumerable<string> errors)
    {
        var errorList = errors.ToList();
        return new(false, errorList.FirstOrDefault(), errorList);
    }
}
