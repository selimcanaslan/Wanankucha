namespace Wanankucha.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string NameSurname { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenEndDate { get; set; }
}
