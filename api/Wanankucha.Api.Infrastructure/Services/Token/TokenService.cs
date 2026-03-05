using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Wanankucha.Api.Application.Abstractions;
using Wanankucha.Api.Application.DTOs;
using Wanankucha.Api.Infrastructure.Options;

namespace Wanankucha.Api.Infrastructure.Services.Token;

public class TokenService : ITokenService
{
    private readonly JwtOptions _jwtOptions;

    public TokenService(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public Application.DTOs.Token CreateAccessToken(UserTokenData userData)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userData.Id.ToString()),
            new Claim(ClaimTypes.Name, userData.UserName),
            new Claim(ClaimTypes.Email, userData.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecurityKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiration = DateTime.UtcNow.AddMinutes(_jwtOptions.Expiration);

        JwtSecurityToken securityToken = new(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiration,
            signingCredentials: credentials);

        JwtSecurityTokenHandler handler = new();
        string tokenString = handler.WriteToken(securityToken);

        return new Application.DTOs.Token
        {
            AccessToken = tokenString,
            Expiration = expiration,
            RefreshToken = CreateRefreshToken()
        };
    }

    public string CreateRefreshToken()
    {
        byte[] number = new byte[32];
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        rng.GetBytes(number);
        return Convert.ToBase64String(number);
    }
}
