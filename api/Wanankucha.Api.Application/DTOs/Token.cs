namespace Wanankucha.Api.Application.DTOs;

public class Token
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime Expiration { get; set; } = DateTime.UtcNow;
    public string RefreshToken { get; set; } = string.Empty;
}