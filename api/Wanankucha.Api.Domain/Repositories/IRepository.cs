using Wanankucha.Api.Domain.Common;

namespace Wanankucha.Api.Domain.Repositories;

public interface IRepository<T> where T : class, IEntity, new()
{
}