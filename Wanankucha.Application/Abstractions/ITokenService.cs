using Wanankucha.Application.DTOs;

namespace Wanankucha.Application.Abstractions;

public interface ITokenService
{
    Token CreateAccessToken(UserDto user);
    string CreateRefreshToken();
}