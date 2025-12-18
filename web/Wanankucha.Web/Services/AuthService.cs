using System.Net.Http.Json;
using Wanankucha.Shared.DTOs;
using Wanankucha.Shared.Wrappers;

namespace Wanankucha.Web.Services;

/// <summary>
/// Authentication service implementation
/// </summary>
public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenStorageService _tokenStorage;

    public AuthService(IHttpClientFactory httpClientFactory, ITokenStorageService tokenStorage)
    {
        _httpClient = httpClientFactory.CreateClient("WanankuchaApi");
        _tokenStorage = tokenStorage;
    }

    public async Task<ApiResponse<TokenDto>> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/Login", request);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<TokenDto>>();
                
                if (result?.Succeeded == true && result.Data != null)
                {
                    await _tokenStorage.SetTokenAsync(result.Data);
                }
                
                return result ?? new ApiResponse<TokenDto>("Invalid response from server");
            }
            
            var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse<TokenDto>>();
            return errorResult ?? new ApiResponse<TokenDto>("Authentication failed");
        }
        catch (Exception ex)
        {
            return new ApiResponse<TokenDto>($"Connection error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<TokenDto>> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            var request = new RefreshTokenRequest { RefreshToken = refreshToken };
            var response = await _httpClient.PostAsJsonAsync("api/Auth/RefreshToken", request);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<TokenDto>>();
                
                if (result?.Succeeded == true && result.Data != null)
                {
                    await _tokenStorage.SetTokenAsync(result.Data);
                }
                
                return result ?? new ApiResponse<TokenDto>("Invalid response from server");
            }
            
            var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse<TokenDto>>();
            return errorResult ?? new ApiResponse<TokenDto>("Token refresh failed");
        }
        catch (Exception ex)
        {
            return new ApiResponse<TokenDto>($"Connection error: {ex.Message}");
        }
    }

    public async Task LogoutAsync()
    {
        await _tokenStorage.ClearTokenAsync();
    }
}
