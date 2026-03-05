using Wanankucha.Api.Domain.Entities;

namespace Wanankucha.Api.Domain.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string normalizedName, CancellationToken cancellationToken = default);
    Task AddAsync(Role role, CancellationToken cancellationToken = default);
    Task AddUserRoleAsync(UserRole userRole, CancellationToken cancellationToken = default);
}
