using MediatR;
using Microsoft.EntityFrameworkCore;
using Wanankucha.Api.Domain.Common;
using Wanankucha.Api.Domain.Entities;

namespace Wanankucha.Api.Persistence.Contexts;

public class AppDbContext : DbContext
{
    private readonly IMediator _mediator;

    public AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator) : base(options)
    {
        _mediator = mediator;
    }

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
        // Get all entities with domain events
        var entitiesWithEvents = ChangeTracker.Entries<IEntity>()
            .Where(e => e.Entity.GetType().GetProperty("DomainEvents") != null)
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = new List<IDomainEvent>();

        foreach (var entity in entitiesWithEvents)
        {
            var property = entity.GetType().GetProperty("DomainEvents");
            if (property == null) continue;

            var events = property.GetValue(entity) as IReadOnlyList<IDomainEvent>;
            if (events != null && events.Any())
            {
                domainEvents.AddRange(events);
                
                // Clear the events so they aren't processed again
                var clearMethod = entity.GetType().GetMethod("ClearDomainEvents");
                clearMethod?.Invoke(entity, null);
            }
        }

        // Save changes to the database
        var result = await base.SaveChangesAsync(cancellationToken);

        // Publish all domain events AFTER saving to the database
        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }

        return result;
    }
}
