namespace Wanankucha.Shared.DTOs;

/// <summary>
/// User registration request model
/// </summary>
public class RegisterRequest
{
    public string NameSurname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PasswordConfirm { get; set; } = string.Empty;
}
