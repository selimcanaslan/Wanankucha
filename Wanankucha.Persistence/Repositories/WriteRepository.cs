using Microsoft.EntityFrameworkCore;
using Wanankucha.Domain.Common;
using Wanankucha.Domain.Repositories;
using Wanankucha.Persistence.Contexts;

namespace Wanankucha.Persistence.Repositories;

public class WriteRepository<T>(AppDbContext context) : IWriteRepository<T>
    where T : class, IEntity, new()
{
    public DbSet<T> Table => context.Set<T>();

    public async Task<bool> AddAsync(T model)
    {
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<T> entityEntry = await Table.AddAsync(model);
        return entityEntry.State == EntityState.Added;
    }

    public async Task<bool> AddRangeAsync(List<T> datas)
    {
        await Table.AddRangeAsync(datas);
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

    public async Task<bool> RemoveAsync(string id)
    {
        T model = (await Table.FirstOrDefaultAsync(data => EF.Property<Guid>(data, "Id") == Guid.Parse(id)))!;
        return Remove(model);
    }

    public bool Update(T model)
    {
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entityEntry = Table.Update(model);
        return entityEntry.State == EntityState.Modified;
    }
}