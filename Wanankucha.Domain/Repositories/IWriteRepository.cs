using Wanankucha.Domain.Common;

namespace Wanankucha.Domain.Repositories;

public interface IWriteRepository<T> : IRepository<T> where T : class, IEntity, new()
{
    Task<bool> AddAsync(T model);
    Task<bool> AddRangeAsync(List<T> datas);

    bool Remove(T model);
    bool RemoveRange(List<T> datas);
    Task<bool> RemoveAsync(string id);
    
    bool Update(T model);
    
    Task<int> SaveAsync();
}