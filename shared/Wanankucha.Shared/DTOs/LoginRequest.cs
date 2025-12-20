using System.ComponentModel.DataAnnotations;

namespace Wanankucha.Shared.DTOs;

/// <summary>
/// Login request model
/// </summary>
public class LoginRequest
{
    [Required(ErrorMessage = "Email or Username is required", AllowEmptyStrings = false)]
    public string EmailOrUserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required", AllowEmptyStrings = false)]
    public string Password { get; set; } = string.Empty;
}
