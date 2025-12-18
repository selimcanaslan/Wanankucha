using System.Linq.Expressions;
using Wanankucha.Api.Domain.Common;

namespace Wanankucha.Api.Domain.Repositories;

public interface IReadRepository<T> : IRepository<T> where T : class, IEntity, new()
{
    IQueryable<T> GetAll(bool tracking = true);
    IQueryable<T> GetWhere(Expression<Func<T, bool>> method, bool tracking = true);
    Task<T?> GetSingleAsync(Expression<Func<T, bool>> method, bool tracking = true, CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(Guid id, bool tracking = true, CancellationToken cancellationToken = default);
}


