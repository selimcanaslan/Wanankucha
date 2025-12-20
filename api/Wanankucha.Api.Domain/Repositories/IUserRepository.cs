using Wanankucha.Api.Domain.Entities;

namespace Wanankucha.Api.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default);
    Task<User?> FindByUsernameAsync(string normalizedUsername, CancellationToken cancellationToken = default);
    Task<User?> FindByEmailOrUsernameAsync(string normalized, CancellationToken cancellationToken = default);
    Task<User?> FindByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> ExistsWithEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default);
    Task<bool> ExistsWithUsernameAsync(string normalizedUsername, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetAllAsync(int page, int size, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    void Update(User user);

    // Role-related methods
    Task<Role?> GetRoleByNameAsync(string name, CancellationToken cancellationToken = default);
    Task AddRoleAsync(Role role, CancellationToken cancellationToken = default);
    Task AddUserRoleAsync(UserRole userRole, CancellationToken cancellationToken = default);
}

