using Wanankucha.Shared.DTOs;
using Wanankucha.Shared.Wrappers;

namespace Wanankucha.Web.Services;

/// <summary>
/// Authentication service for API calls
/// </summary>
public interface IAuthService
{
    Task<ApiResponse<TokenDto>> LoginAsync(LoginRequest request);
    Task<ApiResponse<Guid>> RegisterAsync(RegisterRequest request);
    Task<ApiResponse<TokenDto>> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync();
}
