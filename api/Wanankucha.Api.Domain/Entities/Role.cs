namespace Wanankucha.Api.Domain.Entities;

public class Role : Common.BaseEntity<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string NormalizedName { get; private set; } = string.Empty;

    public virtual ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();

    private Role() { }

    public static Role Create(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new Role
        {
            Id = Guid.NewGuid(),
            Name = name,
            NormalizedName = name.ToUpperInvariant()
        };
    }
}
