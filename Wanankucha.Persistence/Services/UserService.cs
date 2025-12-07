using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wanankucha.Application.Abstractions;
using Wanankucha.Application.DTOs;
using Wanankucha.Application.Wrappers;
using Wanankucha.Persistence.Entities;

namespace Wanankucha.Persistence.Services;

public class UserService(UserManager<AppUser> userManager) : IUserService
{
    public async Task<ServiceResponse<Guid>> CreateUserAsync(string nameSurname, string email, string userName, string password)
    {
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            Email = email,
            NameSurname = nameSurname
        };

        var result = await userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            return new ServiceResponse<Guid>(user.Id, "User created successfully.");
        }

        var errorMessage = result.Errors.FirstOrDefault()?.Description;
        return new ServiceResponse<Guid>(errorMessage ?? "Something went wrong while creating the user.");
    }

    public async Task<UserDto?> FindByEmailOrUsernameAsync(string emailOrUsername)
    {
        var user = await userManager.FindByEmailAsync(emailOrUsername)
                   ?? await userManager.FindByNameAsync(emailOrUsername);

        return user == null ? null : MapToDto(user);
    }

    public async Task<bool> CheckPasswordAsync(Guid userId, string password)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        return await userManager.CheckPasswordAsync(user, password);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(int page, int size)
    {
        var users = await userManager.Users
            .Skip(page * size)
            .Take(size)
            .ToListAsync();

        return users.Select(MapToDto);
    }

    public async Task UpdateRefreshTokenAsync(Guid userId, string refreshToken, DateTime endDate)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null) return;

        user.RefreshToken = refreshToken;
        user.RefreshTokenEndDate = endDate;
        await userManager.UpdateAsync(user);
    }

    public async Task<UserDto?> FindByRefreshTokenAsync(string refreshToken)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        return user == null ? null : MapToDto(user);
    }

    private static UserDto MapToDto(AppUser user)
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
