using Wanankucha.Api.Domain.Common;

namespace Wanankucha.Api.Domain.Repositories;

public interface IWriteRepository<T> : IRepository<T> where T : class, IEntity, new()
{
    Task<bool> AddAsync(T model, CancellationToken cancellationToken = default);
    Task<bool> AddRangeAsync(List<T> datas, CancellationToken cancellationToken = default);

    bool Remove(T model);
    bool RemoveRange(List<T> datas);
    Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default);
    
    bool Update(T model);
}