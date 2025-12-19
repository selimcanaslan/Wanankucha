using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Wanankucha.Shared.DTOs;

namespace Wanankucha.Web.Services;

/// <summary>
/// Secure cookie-based token storage using ProtectedSessionStorage
/// </summary>
public class CookieTokenStorageService(ProtectedSessionStorage sessionStorage) : ITokenStorageService
{
    private const string TokenKey = "auth_token";
    private TokenDto? _cachedToken;

    public async Task<TokenDto?> GetTokenAsync()
    {
        if (_cachedToken != null)
            return _cachedToken;

        try
        {
            var result = await sessionStorage.GetAsync<TokenDto>(TokenKey);
            _cachedToken = result.Success ? result.Value : null;
            return _cachedToken;
        }
        catch
        {
            return null;
        }
    }

    public async Task SetTokenAsync(TokenDto token)
    {
        _cachedToken = token;
        await sessionStorage.SetAsync(TokenKey, token);
    }

    public async Task ClearTokenAsync()
    {
        _cachedToken = null;
        await sessionStorage.DeleteAsync(TokenKey);
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return token != null && token.Expiration > DateTime.UtcNow;
    }
}
