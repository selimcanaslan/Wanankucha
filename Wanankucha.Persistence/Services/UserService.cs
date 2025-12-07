using Microsoft.EntityFrameworkCore;
using Wanankucha.Application.Abstractions;
using Wanankucha.Application.DTOs;
using Wanankucha.Application.Wrappers;
using Wanankucha.Domain.Entities;
using Wanankucha.Persistence.Contexts;

namespace Wanankucha.Persistence.Services;

public class UserService(AppDbContext context, IPasswordHasher passwordHasher) : IUserService
{
    public async Task<ServiceResponse<Guid>> CreateUserAsync(string nameSurname, string email, string userName, string password)
    {
        var normalizedEmail = email.ToUpperInvariant();
        if (await context.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail))
        {
            return new ServiceResponse<Guid>("A user with this email already exists.");
        }

        var normalizedUserName = userName.ToUpperInvariant();
        if (await context.Users.AnyAsync(u => u.NormalizedUserName == normalizedUserName))
        {
            return new ServiceResponse<Guid>("A user with this username already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            NormalizedUserName = normalizedUserName,
            Email = email,
            NormalizedEmail = normalizedEmail,
            NameSurname = nameSurname,
            PasswordHash = passwordHasher.HashPassword(password)
        };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        return new ServiceResponse<Guid>(user.Id, "User created successfully.");
    }

    public async Task<UserDto?> FindByEmailOrUsernameAsync(string emailOrUsername)
    {
        var normalized = emailOrUsername.ToUpperInvariant();
        
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalized || u.NormalizedUserName == normalized);

        return user == null ? null : MapToDto(user);
    }

    public async Task<bool> CheckPasswordAsync(Guid userId, string password)
    {
        var user = await context.Users.FindAsync(userId);
        if (user == null) return false;

        return passwordHasher.VerifyPassword(password, user.PasswordHash);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(int page, int size)
    {
        var users = await context.Users
            .Skip(page * size)
            .Take(size)
            .ToListAsync();

        return users.Select(MapToDto);
    }

    public async Task UpdateRefreshTokenAsync(Guid userId, string refreshToken, DateTime endDate)
    {
        var user = await context.Users.FindAsync(userId);
        if (user == null) return;

        user.RefreshToken = refreshToken;
        user.RefreshTokenEndDate = endDate;
        await context.SaveChangesAsync();
    }

    public async Task<UserDto?> FindByRefreshTokenAsync(string refreshToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        return user == null ? null : MapToDto(user);
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            NameSurname = user.NameSurname,
            RefreshToken = user.RefreshToken,
            RefreshTokenEndDate = user.RefreshTokenEndDate
        };
    }
}

