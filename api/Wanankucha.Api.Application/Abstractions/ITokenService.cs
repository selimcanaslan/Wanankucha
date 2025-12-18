using Wanankucha.Api.Application.DTOs;

namespace Wanankucha.Api.Application.Abstractions;

public interface ITokenService
{
    Token CreateAccessToken(UserDto user);
    string CreateRefreshToken();
}