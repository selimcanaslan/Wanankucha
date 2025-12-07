using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Wanankucha.Domain.Common;
using Wanankucha.Persistence.Entities;

namespace Wanankucha.Persistence.Contexts;

public class AppDbContext(DbContextOptions options) : IdentityDbContext<AppUser, AppRole, Guid>(options)
{
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var data = ChangeTracker.Entries<BaseEntity<Guid>>();

        foreach (var d in data)
        {
            switch (d.State)
            {
                case EntityState.Added:
                    d.Entity.CreatedDate = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    d.Entity.UpdatedDate = DateTime.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}