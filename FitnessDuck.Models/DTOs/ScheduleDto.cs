using FitnessDuck.Data.Entities;
using FitnessDuck.Models.Common;

namespace FitnessDuck.Models.DTOs;

public class ScheduleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Icon { get; set; }
    public string? TrainerId { get; set; }
    public RecurrenceType Recurrence { get; set; }
    public List<DayPlan> WeekPlan { get; set; }
    
    public int DurationMinutes { get; set; }
    public int Seats { get; set; }
    public int MinUnsubscribeHours { get; set; }
    public int AdvanceBookingDays{ get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime? EndDateUtc{ get; set; }
    //public ICollection<LessonDto> Lessons { get; set; }
}