using Microsoft.AspNetCore.Identity;

namespace Wanankucha.Persistence.Entities;

public class AppUser : IdentityUser<Guid>
{
    public string NameSurname { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenEndDate { get; set; }
}
