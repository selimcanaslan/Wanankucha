namespace Wanankucha.Api.Application.DTOs;

/// <summary>
/// Internal DTO used for authentication and token operations.
/// Contains sensitive token fields — do NOT expose this directly in list endpoints.
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string NameSurname { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenEndDate { get; set; }
}
