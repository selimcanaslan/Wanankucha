using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Wanankucha.Api.Domain.Common;

namespace Wanankucha.Api.Persistence.Interceptors;

/// <summary>
/// Automatically stamps CreatedDate and UpdatedDate on entities
/// that implement <see cref="IEntity"/> without requiring the DbContext
/// to know about domain internals.
/// </summary>
public sealed class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        StampAuditFields(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        StampAuditFields(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void StampAuditFields(DbContext? context)
    {
        if (context is null) return;

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity<Guid>>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(nameof(BaseEntity<Guid>.CreatedDate)).CurrentValue = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Property(nameof(BaseEntity<Guid>.UpdatedDate)).CurrentValue = DateTime.UtcNow;
                    break;
            }
        }
    }
}
