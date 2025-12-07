using Wanankucha.Application.DTOs;

namespace Wanankucha.Application.Abstractions;

/// <summary>
/// Service for creating and managing JWT tokens.
/// </summary>
public interface ITokenService
{
    Token CreateAccessToken(UserDto user);
    string CreateRefreshToken();
}