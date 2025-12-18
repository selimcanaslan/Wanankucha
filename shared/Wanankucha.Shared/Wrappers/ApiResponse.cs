namespace Wanankucha.Shared.Wrappers;

/// <summary>
/// Generic API response wrapper
/// </summary>
public class ApiResponse<T>
{
    public T? Data { get; set; }
    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string>? Errors { get; set; }

    public ApiResponse() { }

    public ApiResponse(T data)
    {
        Succeeded = true;
        Data = data;
    }

    public ApiResponse(T data, string message)
    {
        Succeeded = true;
        Message = message;
        Data = data;
    }

    public ApiResponse(string message)
    {
        Succeeded = false;
        Message = message;
    }

    public ApiResponse(List<string> errors)
    {
        Succeeded = false;
        Errors = errors;
    }
}
