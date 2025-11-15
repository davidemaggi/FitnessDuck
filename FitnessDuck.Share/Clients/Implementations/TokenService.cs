using Blazored.LocalStorage;
using FitnessDuck.Share.Providers;

namespace FitnessDuck.Share.Clients.Implementations;

public class TokenService
{
    private readonly ILocalStorageService _localStorage;
    private readonly JwtAuthenticationStateProvider _authStateProvider;

    private const string AccessTokenKey = "authToken";
    private const string RefreshTokenKey = "refreshToken";
    private const string AccessTokenExpiryKey = "accessTokenExpiry";

    public TokenService(ILocalStorageService localStorage, JwtAuthenticationStateProvider authStateProvider)
    {
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
    }

    public async Task<string?> GetAccessTokenAsync() => await _localStorage.GetItemAsync<string>(AccessTokenKey);

    public async Task<DateTime> GetAccessTokenExpiryAsync() => await _localStorage.GetItemAsync<DateTime>(AccessTokenExpiryKey);

    public async Task<string?> GetRefreshTokenAsync() => await _localStorage.GetItemAsync<string>(RefreshTokenKey);

    public async Task StoreTokensAsync(string accessToken, string refreshToken, int expiresInSeconds)
    {
        await _localStorage.SetItemAsync(AccessTokenKey, accessToken);
        await _localStorage.SetItemAsync(RefreshTokenKey, refreshToken);
        await _localStorage.SetItemAsync(AccessTokenExpiryKey, DateTime.UtcNow.AddSeconds(expiresInSeconds - 60));
        _authStateProvider.NotifyUserAuthentication(accessToken);
    }

    public async Task ClearTokensAsync()
    {
        await _localStorage.RemoveItemAsync(AccessTokenKey);
        await _localStorage.RemoveItemAsync(RefreshTokenKey);
        await _localStorage.RemoveItemAsync(AccessTokenExpiryKey);
        _authStateProvider.NotifyUserLogout();
    }
}
