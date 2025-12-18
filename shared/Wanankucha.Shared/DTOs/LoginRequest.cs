namespace Wanankucha.Shared.DTOs;

/// <summary>
/// Login request model
/// </summary>
public class LoginRequest
{
    public string EmailOrUserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
