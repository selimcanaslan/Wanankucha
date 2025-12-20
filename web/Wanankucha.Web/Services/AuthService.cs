using System.Net;
using System.Net.Http.Json;
using Polly.CircuitBreaker;
using Wanankucha.Shared.DTOs;
using Wanankucha.Shared.Wrappers;

namespace Wanankucha.Web.Services;

/// <summary>
/// Authentication service implementation
/// </summary>
public class AuthService(IHttpClientFactory httpClientFactory, ITokenStorageService tokenStorage)
    : IAuthService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("WanankuchaApi");

    public async Task<ApiResponse<TokenDto>> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/v1/Auth/Login", request);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<TokenDto>>();
                
                if (result is { Succeeded: true, Data: not null })
                {
                    await tokenStorage.SetTokenAsync(result.Data);
                }
                
                return result ?? new ApiResponse<TokenDto>("Invalid response from server");
            }
            
            // Handle rate limiting (429) - returns plain text, not JSON
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                var message = await response.Content.ReadAsStringAsync();
                return new ApiResponse<TokenDto>(message);
            }
            
            // Try to parse JSON error response
            try
            {
                var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse<TokenDto>>();
                return errorResult ?? new ApiResponse<TokenDto>("Authentication failed");
            }
            catch
            {
                // Fallback for non-JSON error responses
                var errorText = await response.Content.ReadAsStringAsync();
                return new ApiResponse<TokenDto>(string.IsNullOrEmpty(errorText) ? "Authentication failed" : errorText);
            }
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            return new ApiResponse<TokenDto>("Request timed out. Please check your connection and try again.");
        }
        catch (TaskCanceledException)
        {
            return new ApiResponse<TokenDto>("Request timed out. Please try again.");
        }
        catch (BrokenCircuitException)
        {
            return new ApiResponse<TokenDto>("Service is temporarily unavailable. Please try again in a few moments.");
        }
        catch (HttpRequestException ex)
        {
            return new ApiResponse<TokenDto>($"Network error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return new ApiResponse<TokenDto>($"Connection error: {ex.Message}");
        }
    }


    public async Task<ApiResponse<Guid>> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/v1/Auth/Register", request);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>();
                return result ?? new ApiResponse<Guid>("Invalid response from server");
            }
            
            // Handle rate limiting (429) - returns plain text, not JSON
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                var message = await response.Content.ReadAsStringAsync();
                return new ApiResponse<Guid>(message);
            }
            
            // Try to parse JSON error response
            try
            {
                var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>();
                return errorResult ?? new ApiResponse<Guid>("Registration failed");
            }
            catch
            {
                var errorText = await response.Content.ReadAsStringAsync();
                return new ApiResponse<Guid>(string.IsNullOrEmpty(errorText) ? "Registration failed" : errorText);
            }
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            return new ApiResponse<Guid>("Request timed out. Please check your connection and try again.");
        }
        catch (TaskCanceledException)
        {
            return new ApiResponse<Guid>("Request timed out. Please try again.");
        }
        catch (BrokenCircuitException)
        {
            return new ApiResponse<Guid>("Service is temporarily unavailable. Please try again in a few moments.");
        }
        catch (HttpRequestException ex)
        {
            return new ApiResponse<Guid>($"Network error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return new ApiResponse<Guid>($"Connection error: {ex.Message}");
        }
    }


    public async Task<ApiResponse<TokenDto>> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            var request = new RefreshTokenRequest { RefreshToken = refreshToken };
            var response = await _httpClient.PostAsJsonAsync("api/v1/Auth/RefreshToken", request);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<TokenDto>>();
                
                if (result?.Succeeded == true && result.Data != null)
                {
                    await tokenStorage.SetTokenAsync(result.Data);
                }
                
                return result ?? new ApiResponse<TokenDto>("Invalid response from server");
            }
            
            var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse<TokenDto>>();
            return errorResult ?? new ApiResponse<TokenDto>("Token refresh failed");
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            return new ApiResponse<TokenDto>("Request timed out. Please check your connection and try again.");
        }
        catch (TaskCanceledException)
        {
            return new ApiResponse<TokenDto>("Request timed out. Please try again.");
        }
        catch (BrokenCircuitException)
        {
            return new ApiResponse<TokenDto>("Service is temporarily unavailable. Please try again in a few moments.");
        }
        catch (HttpRequestException ex)
        {
            return new ApiResponse<TokenDto>($"Network error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return new ApiResponse<TokenDto>($"Connection error: {ex.Message}");
        }
    }

    public async Task LogoutAsync()
    {
        await tokenStorage.ClearTokenAsync();
    }
}
