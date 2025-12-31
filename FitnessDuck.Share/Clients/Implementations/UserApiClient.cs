using System.Net.Http.Json;
using Blazored.LocalStorage;
using FitnessDuck.Models;
using FitnessDuck.Models.DTOs;
using FitnessDuck.Share.Providers;

namespace FitnessDuck.Share.Clients.Implementations;

public class UserApiClient
{
    private readonly HttpClient _httpClient;

    public UserApiClient(HttpClient httpClient, TokenService tokenService)
    {
        _httpClient = httpClient;
        Console.WriteLine($"BaseAddress: {_httpClient.BaseAddress}");
    }

    public async Task<IEnumerable<UserDto>> GetAll()
    {
        var response = await _httpClient.GetAsync("api/Users");
        if(!response.IsSuccessStatusCode) return new List<UserDto>();

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<UserDto>>();
        return result;
    }
    
    public async Task<UserDto?> Save(UserDto userDto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Users",userDto);
        if(!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<UserDto>();
        return result;
    }
    
}

