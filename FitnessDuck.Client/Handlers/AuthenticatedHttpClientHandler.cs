using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using FitnessDuck.Models.DTOs;
using FitnessDuck.Share.Clients.Implementations;

namespace FitnessDuck.Client.Handlers;

public class AuthenticatedHttpClientHandler : DelegatingHandler
{
    private readonly TokenService _tokenService;
    private readonly HttpClient _httpClientForRefresh;
    private readonly ILogger<AuthenticatedHttpClientHandler> _logger;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private bool _refreshing;

    public AuthenticatedHttpClientHandler(TokenService tokenService, IHttpClientFactory httpClientFactory, ILogger<AuthenticatedHttpClientHandler> logger)
    {
        _tokenService = tokenService;
        _logger = logger;
        // Separate HttpClient instance for refresh calls to avoid recursion in this handler
        _httpClientForRefresh = httpClientFactory.CreateClient("RefreshClient");
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = await _tokenService.GetAccessTokenAsync();

        if (!string.IsNullOrWhiteSpace(accessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await _refreshLock.WaitAsync(cancellationToken);
            try
            {
                if (!_refreshing)
                {
                    _refreshing = true;
                    _logger.LogInformation("Token expired, attempting refresh...");

                    var refreshed = await RefreshTokenAsync();

                    if (refreshed)
                    {
                        accessToken = await _tokenService.GetAccessTokenAsync();

                        // Retry original request with new token
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                        response.Dispose(); // Dispose old response

                        response = await base.SendAsync(request, cancellationToken);
                    }
                    else
                    {
                        _logger.LogWarning("Token refresh failed.");
                    }
                }
            }
            finally
            {
                _refreshing = false;
                _refreshLock.Release();
            }
        }

        return response;
    }

    private async Task<bool> RefreshTokenAsync()
    {
        var refreshToken = await _tokenService.GetRefreshTokenAsync();
        if (string.IsNullOrEmpty(refreshToken))
            return false;

        var refreshRequest = new
        {
            RefreshToken = refreshToken
        };

        try
        {
            var response = await _httpClientForRefresh.PostAsJsonAsync("api/Auth/refresh", refreshRequest);

            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadFromJsonAsync<ValidateTokenResponseDto>();
            if (result == null)
                return false;

            await _tokenService.StoreTokensAsync(result.AccessToken, result.RefreshToken, result.ExpiresInSeconds);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return false;
        }
    }
}