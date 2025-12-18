using Microsoft.EntityFrameworkCore;
using Wanankucha.Api.Domain.Common;
using Wanankucha.Api.Domain.Repositories;
using Wanankucha.Api.Persistence.Contexts;

namespace Wanankucha.Api.Persistence.Repositories;

public class WriteRepository<T>(AppDbContext context) : IWriteRepository<T>
    where T : class, IEntity, new()
{
    public DbSet<T> Table => context.Set<T>();

    public async Task<bool> AddAsync(T model, CancellationToken cancellationToken = default)
    {
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<T> entityEntry = await Table.AddAsync(model, cancellationToken);
        return entityEntry.State == EntityState.Added;
    }

    public async Task<bool> AddRangeAsync(List<T> datas, CancellationToken cancellationToken = default)
    {
        await Table.AddRangeAsync(datas, cancellationToken);
        return true;
    }

    public bool Remove(T model)
    {
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<T> entityEntry = Table.Remove(model);
        return entityEntry.State == EntityState.Deleted;
    }

    public bool RemoveRange(List<T> datas)
    {
        Table.RemoveRange(datas);
        return true;
    }

    public async Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        T? model = await Table.FirstOrDefaultAsync(data => EF.Property<Guid>(data, "Id") == id, cancellationToken);
        return model != null && Remove(model);
    }

    public bool Update(T model)
    {
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entityEntry = Table.Update(model);
        return entityEntry.State == EntityState.Modified;
    }
}