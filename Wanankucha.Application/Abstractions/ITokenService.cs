using Wanankucha.Application.DTOs;
using Wanankucha.Domain.Entities.Identity;

namespace Wanankucha.Application.Abstractions;

public interface ITokenService
{
    Token CreateAccessToken(AppUser user);
    string CreateRefreshToken();
}