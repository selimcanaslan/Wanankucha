using Microsoft.AspNetCore.Identity;
using Wanankucha.Domain.Common;

namespace Wanankucha.Domain.Entities.Identity;

public class AppUser : IdentityUser<Guid>, IEntity
{
    public string NameSurname { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenEndDate { get; set; }
}