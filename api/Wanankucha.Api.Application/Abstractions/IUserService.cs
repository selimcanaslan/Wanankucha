using Wanankucha.Api.Application.DTOs;
using Wanankucha.Api.Application.Wrappers;

namespace Wanankucha.Api.Application.Abstractions;

public interface IUserService
{
    Task<ServiceResponse<Guid>> CreateUserAsync(string nameSurname, string email, string userName, string password, CancellationToken cancellationToken = default);
    Task<UserDto?> FindByEmailOrUsernameAsync(string emailOrUsername, CancellationToken cancellationToken = default);
    Task<bool> CheckPasswordAsync(Guid userId, string password, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserDto>> GetAllUsersAsync(int page, int size, CancellationToken cancellationToken = default);
    Task UpdateRefreshTokenAsync(Guid userId, string refreshToken, DateTime endDate, CancellationToken cancellationToken = default);
    Task<UserDto?> FindByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}
