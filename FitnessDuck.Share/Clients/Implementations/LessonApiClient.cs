using System.Net.Http.Json;
using Blazored.LocalStorage;
using FitnessDuck.Models;
using FitnessDuck.Models.DTOs;
using FitnessDuck.Share.Providers;

namespace FitnessDuck.Share.Clients.Implementations;

public class LessonApiClient
{
    private readonly HttpClient _httpClient;

    public LessonApiClient(HttpClient httpClient, TokenService tokenService)
    {
        _httpClient = httpClient;
        Console.WriteLine($"BaseAddress: {_httpClient.BaseAddress}");
    }

    public async Task<IEnumerable<LessonDto>> GetUpcomingLessons()
    {
        var response = await _httpClient.GetAsync("api/Lessons/upcoming");
        if(!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<LessonDto>>();
        return result;
    }
    
    public async Task<IEnumerable<LessonDto>> GetUpcomingLessons(DateTime? from, DateTime? to)
    {
        
        var urlWithParams = $"api/Lessons/upcoming";
        if (from is not null && to is not null)
        {
            
            var queryParams = new Dictionary<string, string?>
            {
                ["fromStr"] = from?.ToString(),
                ["toStr"] = to?.ToString()
            };
           var queryString = new FormUrlEncodedContent(queryParams).ReadAsStringAsync().Result; // or await
            
            urlWithParams = $"{urlWithParams}?{queryString}";

        }
   
        var response = await _httpClient.GetAsync(urlWithParams);
        if(!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<LessonDto>>();
        return result;
    }


    public async Task<IEnumerable<LessonDto>> GetLessonForDay(DateTime selectedDay) => await
        GetUpcomingLessons(selectedDay, selectedDay.AddDays(1).AddMilliseconds(-1));
    
    
    
    public async Task<LessonDto> SubscribeToLesson(Guid lessonId, Guid userId)
    {
        
        var urlWithParams = $"api/Lessons/subscribe";

        
        var queryParams = new Dictionary<string, string?>
        {
            ["userId"] = userId.ToString(),
            ["lessonId"] = lessonId.ToString()
        };
        
        var queryString = new FormUrlEncodedContent(queryParams).ReadAsStringAsync().Result; // or await
        urlWithParams = $"{urlWithParams}?{queryString}";

        var response = await _httpClient.PutAsync(urlWithParams,null);
        if(!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<LessonDto>();
        return result;
    }


    public async Task<LessonDto>  UnsubscribeFromLesson(Guid lessonId, Guid userId)
    {
        var urlWithParams = $"api/Lessons/subscribe";

        
        var queryParams = new Dictionary<string, string?>
        {
            ["userId"] = userId.ToString(),
            ["lessonId"] = lessonId.ToString()
        };
        
        var queryString = new FormUrlEncodedContent(queryParams).ReadAsStringAsync().Result; // or await
        urlWithParams = $"{urlWithParams}?{queryString}";

        var response = await _httpClient.DeleteAsync(urlWithParams);
        if(!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<LessonDto>();
        return result;    
    }
}

