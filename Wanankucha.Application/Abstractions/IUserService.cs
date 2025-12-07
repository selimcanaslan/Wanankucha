using Wanankucha.Application.DTOs;
using Wanankucha.Application.Wrappers;

namespace Wanankucha.Application.Abstractions;

public interface IUserService
{
    Task<ServiceResponse<Guid>> CreateUserAsync(string nameSurname, string email, string userName, string password);
    Task<UserDto?> FindByEmailOrUsernameAsync(string emailOrUsername);
    Task<bool> CheckPasswordAsync(Guid userId, string password);
    Task<IEnumerable<UserDto>> GetAllUsersAsync(int page, int size);
    Task UpdateRefreshTokenAsync(Guid userId, string refreshToken, DateTime endDate);
    Task<UserDto?> FindByRefreshTokenAsync(string refreshToken);
}
