using System.Net.Http.Json;
using Blazored.LocalStorage;
using FitnessDuck.Models;
using FitnessDuck.Models.DTOs;
using FitnessDuck.Share.Providers;

namespace FitnessDuck.Share.Clients.Implementations;

public class AuthApiClient
{
    private readonly HttpClient _httpClient;
    private readonly TokenService _tokenService;

    public AuthApiClient(HttpClient httpClient, TokenService tokenService)
    {
        _httpClient = httpClient;
        _tokenService = tokenService;
        Console.WriteLine($"BaseAddress: {_httpClient.BaseAddress}");
    }

    public async Task<bool> RequestTokenAsync(RequestTokenDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Auth/request-token", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<UserDto?> MeAsync()
    {
        var response = await _httpClient.GetAsync("api/Auth/me");
        if(!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<UserDto>();
    }

    public async Task<bool> LoginAsync(string contact, ContactMethod contactMethod, string token)
    {
        var loginRequest = new ValidateTokenDto
        {
            ContactMethod = contactMethod,
            Contact = contact,
            Token = token
        };

        var response = await _httpClient.PostAsJsonAsync("api/Auth/validate-token", loginRequest);
        if(!response.IsSuccessStatusCode) return false;

        var result = await response.Content.ReadFromJsonAsync<ValidateTokenResponseDto>();
        await _tokenService.StoreTokensAsync(result.AccessToken, result.RefreshToken, result.ExpiresInSeconds);

        return true;
    }

    public async Task<bool> RefreshTokenAsync()
    {
        var refreshToken = await _tokenService.GetRefreshTokenAsync();
        var refreshRequest = new { RefreshToken = refreshToken };

        var response = await _httpClient.PostAsJsonAsync("api/Auth/refresh", refreshRequest);
        if(!response.IsSuccessStatusCode) return false;

        var result = await response.Content.ReadFromJsonAsync<ValidateTokenResponseDto>();
        await _tokenService.StoreTokensAsync(result.AccessToken, result.RefreshToken, result.ExpiresInSeconds);
        return true;
    }
    public async Task<UserDto?> UpdateUserInfo(UserInfoDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Auth/update-user-info", dto);
        if(!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<UserDto>();
        return result;
    }
    
    
    
    
    public async Task LogoutAsync()
    {
        await _tokenService.ClearTokensAsync();
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public async Task EnsureTokenValidAsync()
    {
        var expiry = await _tokenService.GetAccessTokenExpiryAsync();
        if(DateTime.UtcNow >= expiry)
            await RefreshTokenAsync();
    }

    public async Task<T> GetProtectedDataAsync<T>(string endpoint)
    {
        await EnsureTokenValidAsync();
        var response = await _httpClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }
}

