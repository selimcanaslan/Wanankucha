namespace Wanankucha.Api.Domain.Entities;

public class UserRole : Common.BaseEntity<Guid>
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    
    public Guid RoleId { get; set; }
    public virtual Role Role { get; set; } = null!;
}
