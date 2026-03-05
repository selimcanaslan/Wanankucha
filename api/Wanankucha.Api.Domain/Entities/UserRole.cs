namespace Wanankucha.Api.Domain.Entities;

public class UserRole : Common.BaseEntity<Guid>
{
    public Guid UserId { get; private set; }
    public virtual User User { get; private set; } = null!;

    public Guid RoleId { get; private set; }
    public virtual Role Role { get; private set; } = null!;

    private UserRole() { }

    public static UserRole Create(Guid userId, Guid roleId)
    {
        return new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId
        };
    }
}
