using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Wanankucha.Application.Abstractions;
using Wanankucha.Domain.Entities.Identity;

namespace Wanankucha.Infrastructure.Services.Token;

public class TokenService : ITokenService
{
    
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Wanankucha.Application.DTOs.Token CreateAccessToken(AppUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName ?? ""),
            new Claim(ClaimTypes.Email, user.Email ?? "")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Token:SecurityKey"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiration = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Token:Expiration"]));

        // 4. Token Oluşturma
        JwtSecurityToken securityToken = new(
            issuer: _configuration["Token:Issuer"],
            audience: _configuration["Token:Audience"],
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiration,
            signingCredentials: credentials);

        JwtSecurityTokenHandler handler = new();
        string tokenString = handler.WriteToken(securityToken);

        return new Wanankucha.Application.DTOs.Token
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