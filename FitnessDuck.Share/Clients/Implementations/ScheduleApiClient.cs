using System.Net.Http.Json;
using Blazored.LocalStorage;
using FitnessDuck.Models;
using FitnessDuck.Models.DTOs;
using FitnessDuck.Share.Providers;

namespace FitnessDuck.Share.Clients.Implementations;

public class ScheduleApiClient
{
    private readonly HttpClient _httpClient;

    public ScheduleApiClient(HttpClient httpClient, TokenService tokenService)
    {
        _httpClient = httpClient;
        Console.WriteLine($"BaseAddress: {_httpClient.BaseAddress}");
    }

    public async Task<IEnumerable<LessonDto>> GetUpcomingLessons()
    {
        var response = await _httpClient.GetAsync("api/Schedules/upcoming");
        if(!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<LessonDto>>();
        return result;
    }
    
    public async Task<IEnumerable<ScheduleDto>> GetSchedules()
    {
        
        var urlWithParams = $"api/Schedules";
       /* if (from is not null && to is not null)
        {
            
            var queryParams = new Dictionary<string, string?>
            {
                ["fromStr"] = from?.ToString(),
                ["toStr"] = to?.ToString()
            };
           var queryString = new FormUrlEncodedContent(queryParams).ReadAsStringAsync().Result; // or await
            
            urlWithParams = $"{urlWithParams}?{queryString}";

        }*/
   
        var response = await _httpClient.GetAsync(urlWithParams);
        if(!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<ScheduleDto>>();
        return result;
    }


  
    public async Task<ScheduleDto> SaveSchedule(ScheduleDto dto)
    {
        
     var toSave = new SaveScheduleDto
     {
         Id = dto.Id,
         Name = dto.Name,
         Description = dto.Description,
         Icon = dto.Icon,
         TrainerId = dto.TrainerId,
         Recurrence = dto.Recurrence,
         WeekPlan = dto.WeekPlan,
         DurationMinutes = dto.DurationMinutes,
         Seats = dto.Seats,
         MinUnsubscribeHours = dto.MinUnsubscribeHours,
         AdvanceBookingDays = dto.AdvanceBookingDays,
         StartDateUtc = dto.StartDateUtc,
         EndDateUtc = dto.EndDateUtc,
     };

       
        return await SaveSchedule(toSave);
    }
    
    
    public async Task<ScheduleDto> SaveSchedule(SaveScheduleDto dto)
    {
        
    

        var response = await _httpClient.PostAsJsonAsync("api/Schedules",dto);
        if(!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<ScheduleDto>();
        return result;
    }


    
}

