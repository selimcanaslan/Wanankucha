using Microsoft.EntityFrameworkCore;
using Wanankucha.Api.Domain.Common;
using Wanankucha.Api.Domain.Entities;

namespace Wanankucha.Api.Persistence.Contexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        
        // Global query filters for soft delete
        modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Role>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<UserRole>().HasQueryFilter(e => !e.IsDeleted);
    }

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