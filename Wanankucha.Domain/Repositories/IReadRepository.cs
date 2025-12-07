using System.Linq.Expressions;
using Wanankucha.Domain.Common;

namespace Wanankucha.Domain.Repositories;

public interface IReadRepository<T> : IRepository<T> where T : class, IEntity, new()
{
    IQueryable<T> GetAll(bool tracking = true);
    IQueryable<T> GetWhere(Expression<Func<T, bool>> method, bool tracking = true);
    Task<T?> GetSingleAsync(Expression<Func<T, bool>> method, bool tracking = true);
    Task<T?> GetByIdAsync(string id, bool tracking = true);
}

