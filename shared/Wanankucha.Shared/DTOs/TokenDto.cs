namespace Wanankucha.Shared.DTOs;

/// <summary>
/// Token response containing access and refresh tokens
/// </summary>
public class TokenDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime Expiration { get; set; } = DateTime.UtcNow;
    public string RefreshToken { get; set; } = string.Empty;
}
