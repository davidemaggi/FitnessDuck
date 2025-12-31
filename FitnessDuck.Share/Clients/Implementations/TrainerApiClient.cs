using System.Net.Http.Json;
using Blazored.LocalStorage;
using FitnessDuck.Models;
using FitnessDuck.Models.DTOs;
using FitnessDuck.Share.Providers;

namespace FitnessDuck.Share.Clients.Implementations;

public class TrainerApiClient
{
    private readonly HttpClient _httpClient;

    public TrainerApiClient(HttpClient httpClient, TokenService tokenService)
    {
        _httpClient = httpClient;
        Console.WriteLine($"BaseAddress: {_httpClient.BaseAddress}");
    }

    public async Task<IEnumerable<UserInfoDto>> GetAllTrainers()
    {
        var response = await _httpClient.GetAsync("api/Trainers");
        if(!response.IsSuccessStatusCode) return new List<UserInfoDto>();

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<UserInfoDto>>();
        return result;
    }
    
}

