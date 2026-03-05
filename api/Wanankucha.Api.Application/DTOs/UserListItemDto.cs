namespace Wanankucha.Api.Application.DTOs;

/// <summary>
/// Lightweight DTO for listing users. Does NOT include sensitive token data.
/// </summary>
public class UserListItemDto
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string NameSurname { get; set; } = string.Empty;
}
