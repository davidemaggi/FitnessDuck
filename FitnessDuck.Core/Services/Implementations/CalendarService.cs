using FitnessData.Core.Services.Interfaces;
using FitnessDuck.Models.DTOs;

namespace FitnessDuck.Core.Services.Implementations;

public class CalendarService:ICalendardService
{
    public  CalendarService()
    {
       
    }

    public List<CalendarDayDto> GetCaleendarView(DateTime? from)
    {
        
        DateTime fr= from is null ? DateTime.Today : from.Value;
        
        var ret= new List<CalendarDayDto>();
        
        
        
        
        
   var _days = Enumerable.Range(0, 7).Select(i => fr.AddDays(i)).ToList();

   foreach (var day in _days)
   {
       ret.Add(new CalendarDayDto(day));
   }



   return ret;

    }

    public DateTime GetLastMonday()
    {
        DateTime today = DateTime.Today; 
        int diff = (7 + (DateTime.Today.DayOfWeek - DayOfWeek.Monday)) % 7;
        DateTime monday = today.AddDays(-diff);
        
        return monday;
    }
}