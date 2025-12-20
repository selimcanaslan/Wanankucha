using Wanankucha.Api.Application.DTOs;
using Wanankucha.Api.Domain.Entities;

namespace Wanankucha.Api.Application.Abstractions;

public interface ITokenService
{
    Token CreateAccessToken(User user);
    string CreateRefreshToken();
}