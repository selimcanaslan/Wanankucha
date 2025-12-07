using Microsoft.EntityFrameworkCore;
using Wanankucha.Domain.Entities;
using Wanankucha.Domain.Repositories;
using Wanankucha.Persistence.Contexts;

namespace Wanankucha.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) => _context = context;

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Users.FindAsync(new object[] { id }, cancellationToken);

    public async Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
        => await _context.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);

    public async Task<User?> FindByUsernameAsync(string normalizedUsername, CancellationToken cancellationToken = default)
        => await _context.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUsername, cancellationToken);

    public async Task<User?> FindByEmailOrUsernameAsync(string normalized, CancellationToken cancellationToken = default)
        => await _context.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalized || u.NormalizedUserName == normalized, cancellationToken);

    public async Task<User?> FindByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        => await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, cancellationToken);

    public async Task<bool> ExistsWithEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
        => await _context.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);

    public async Task<bool> ExistsWithUsernameAsync(string normalizedUsername, CancellationToken cancellationToken = default)
        => await _context.Users.AnyAsync(u => u.NormalizedUserName == normalizedUsername, cancellationToken);

    public async Task<IEnumerable<User>> GetAllAsync(int page, int size, CancellationToken cancellationToken = default)
        => await _context.Users
            .Skip(page * size)
            .Take(size)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        => await _context.Users.AddAsync(user, cancellationToken);

    public void Update(User user)
        => _context.Users.Update(user);
}
