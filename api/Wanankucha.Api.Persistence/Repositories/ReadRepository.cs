using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Wanankucha.Api.Domain.Common;
using Wanankucha.Api.Domain.Repositories;
using Wanankucha.Api.Persistence.Contexts;

namespace Wanankucha.Api.Persistence.Repositories;

public class ReadRepository<T>(AppDbContext context) : IReadRepository<T> where T : class, IEntity, new()
{
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

    public async Task<T?> GetSingleAsync(Expression<Func<T, bool>> method, bool tracking = true, CancellationToken cancellationToken = default)
    {
        var query = Table.AsQueryable();
        if (!tracking)
            query = query.AsNoTracking();
        return await query.FirstOrDefaultAsync(method, cancellationToken);
    }

    public async Task<T?> GetByIdAsync(Guid id, bool tracking = true, CancellationToken cancellationToken = default)
    {
        var query = Table.AsQueryable();
        if(!tracking)
            query = query.AsNoTracking();
                
        return await query.FirstOrDefaultAsync(data => EF.Property<Guid>(data, "Id") == id, cancellationToken);
    }
}