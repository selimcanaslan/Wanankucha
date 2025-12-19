using System.ComponentModel.DataAnnotations;

namespace Wanankucha.Shared.DTOs;

/// <summary>
/// Request for initiating a password reset
/// </summary>
public class ForgotPasswordRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;
}
