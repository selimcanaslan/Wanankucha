namespace Wanankucha.Domain.Entities;

public class User : Common.BaseEntity<Guid>
{
    public string UserName { get; set; } = string.Empty;
    public string NormalizedUserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NormalizedEmail { get; set; } = string.Empty;
    public string NameSurname { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenEndDate { get; set; }
    
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
