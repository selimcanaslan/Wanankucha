using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Wanankucha.Web.Auth;

/// <summary>
/// Custom authentication state provider using JWT tokens
/// </summary>
public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly Services.ITokenStorageService _tokenStorage;
    private readonly Services.IAuthService _authService;

    public JwtAuthenticationStateProvider(
        Services.ITokenStorageService tokenStorage,
        Services.IAuthService authService)
    {
        _tokenStorage = tokenStorage;
        _authService = authService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _tokenStorage.GetTokenAsync();

            if (token == null || string.IsNullOrEmpty(token.AccessToken))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            // Check if token is expired
            if (token.Expiration <= DateTime.UtcNow)
            {
                // Try to refresh the token
                if (!string.IsNullOrEmpty(token.RefreshToken))
                {
                    var refreshResult = await _authService.RefreshTokenAsync(token.RefreshToken);
                    if (refreshResult.Succeeded && refreshResult.Data != null)
                    {
                        token = refreshResult.Data;
                    }
                    else
                    {
                        await _tokenStorage.ClearTokenAsync();
                        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                    }
                }
                else
                {
                    await _tokenStorage.ClearTokenAsync();
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }
            }

            var claims = ParseClaimsFromJwt(token.AccessToken);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }
        catch
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    public void NotifyAuthenticationStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        return token.Claims;
    }
}
