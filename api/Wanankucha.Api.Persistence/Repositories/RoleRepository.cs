using Microsoft.EntityFrameworkCore;
using Wanankucha.Api.Domain.Entities;
using Wanankucha.Api.Domain.Repositories;
using Wanankucha.Api.Persistence.Contexts;

namespace Wanankucha.Api.Persistence.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _context;

    public RoleRepository(AppDbContext context) => _context = context;

    public async Task<Role?> GetByNameAsync(string normalizedName, CancellationToken cancellationToken = default)
        => await _context.Roles
            .FirstOrDefaultAsync(r => r.NormalizedName == normalizedName, cancellationToken);

    public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
        => await _context.Roles.AddAsync(role, cancellationToken);

    public async Task AddUserRoleAsync(UserRole userRole, CancellationToken cancellationToken = default)
        => await _context.UserRoles.AddAsync(userRole, cancellationToken);
}
