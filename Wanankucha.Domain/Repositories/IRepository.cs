using Wanankucha.Domain.Common;

namespace Wanankucha.Domain.Repositories;

public interface IRepository<T> where T : class, IEntity, new()
{
}