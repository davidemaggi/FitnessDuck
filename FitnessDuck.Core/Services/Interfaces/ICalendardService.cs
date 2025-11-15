using FitnessDuck.Models.DTOs;

namespace FitnessData.Core.Services.Interfaces;

public interface ICalendardService
{
    List<CalendarDayDto> GetCaleendarView(DateTime? from);
    DateTime GetLastMonday();
}