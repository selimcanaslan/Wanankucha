using Wanankucha.Shared.DTOs;

namespace Wanankucha.Web.Services;

/// <summary>
/// Token storage service interface
/// </summary>
public interface ITokenStorageService
{
    Task<TokenDto?> GetTokenAsync();
    Task SetTokenAsync(TokenDto token);
    Task ClearTokenAsync();
    Task<bool> IsAuthenticatedAsync();
}
