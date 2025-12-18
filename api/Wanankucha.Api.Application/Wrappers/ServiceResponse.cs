namespace Wanankucha.Api.Application.Wrappers;

public class ServiceResponse<T>
{
    public T? Data { get; set; }
    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string>? Errors { get; set; }

    public ServiceResponse(T data)
    {
        Succeeded = true;
        Message = string.Empty;
        Errors = null;
        Data = data;
    }

    public ServiceResponse(T data, string message)
    {
        Succeeded = true;
        Message = message;
        Data = data;
        Errors = null;
    }

    public ServiceResponse(string message)
    {
        Succeeded = false;
        Message = message;
        Data = default;
        Errors = null;
    }

    public ServiceResponse() { }
}