using Wanankucha.Application.DTOs;
using Wanankucha.Application.Wrappers;

namespace Wanankucha.Application.Abstractions;

/// <summary>
/// Abstraction for user-related operations.
/// Decouples the Application layer from ASP.NET Core Identity.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Creates a new user.
    /// </summary>
    Task<ServiceResponse<Guid>> CreateUserAsync(string nameSurname, string email, string userName, string password);
    
    /// <summary>
    /// Finds a user by email or username.
    /// </summary>
    Task<UserDto?> FindByEmailOrUsernameAsync(string emailOrUsername);
    
    /// <summary>
    /// Checks if the password is valid for the given user.
    /// </summary>
    Task<bool> CheckPasswordAsync(Guid userId, string password);
    
    /// <summary>
    /// Gets all users with pagination.
    /// </summary>
    Task<IEnumerable<UserDto>> GetAllUsersAsync(int page, int size);
    
    /// <summary>
    /// Updates the refresh token for a user.
    /// </summary>
    Task UpdateRefreshTokenAsync(Guid userId, string refreshToken, DateTime endDate);
    
    /// <summary>
    /// Finds a user by their refresh token.
    /// </summary>
    Task<UserDto?> FindByRefreshTokenAsync(string refreshToken);
}
