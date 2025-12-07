using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Wanankucha.Domain.Common;
using Wanankucha.Domain.Repositories;
using Wanankucha.Persistence.Contexts;

namespace Wanankucha.Persistence.Repositories;

public class ReadRepository<T>(AppDbContext context) : IReadRepository<T> where T : class, IEntity, new()
{
    // Context.Set<T>() her seferinde yazmamak için bir property.
    public DbSet<T> Table => context.Set<T>();

    public IQueryable<T> GetAll(bool tracking = true)
    {
        var query = Table.AsQueryable();
        if (!tracking)
            query = query.AsNoTracking();
        return query;
    }

    public IQueryable<T> GetWhere(Expression<Func<T, bool>> method, bool tracking = true)
    {
        var query = Table.Where(method);
        if (!tracking)
            query = query.AsNoTracking();
        return query;
    }

    public async Task<T> GetSingleAsync(Expression<Func<T, bool>> method, bool tracking = true)
    {
        var query = Table.AsQueryable();
        if (!tracking)
            query = query.AsNoTracking();
        return await query.FirstOrDefaultAsync(method) ?? new T();
    }

    public async Task<T> GetByIdAsync(string id, bool tracking = true)
    {
        // FindAsync primary key üzerinden arama yapar.
        // Ancak tracking mekanizmasını kontrol etmek istersek query kullanmalıyız.
        // Marker interface (IEntity) kullandığımız için x.Id diyemiyoruz, 
        // ama query mantığıyla Reflection veya Marker pattern kullanılabilir.
        // Pratik ve Best Practice çözüm:
            
        var query = Table.AsQueryable();
        if(!tracking)
            query = query.AsNoTracking();
                
        // Not: BaseEntity<Guid> kullandığımızı varsayarak Guid.Parse yapıyoruz.
        return await query.FirstOrDefaultAsync(data => EF.Property<Guid>(data, "Id") == Guid.Parse(id)) ?? new T();
    }
}