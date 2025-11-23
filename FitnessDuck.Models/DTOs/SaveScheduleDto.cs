using FitnessDuck.Data.Entities;
using FitnessDuck.Models.Common;

namespace FitnessDuck.Models.DTOs;

public class SaveScheduleDto
{
    public Guid? Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public Guid? TrainerId { get; set; }
    public RecurrenceType Recurrence { get; set; } = RecurrenceType.WeekPlan;
    public List<DayPlan> WeekPlan { get; set; } = DayPlan.BuildWeekPlan();
    
    public int DurationMinutes { get; set; } = 60;
        
    public int Seats { get; set; } = 1;
    public int MinUnsubscribeHours { get; set; } = 24;
    public int AdvanceBookingDays { get; set; } = 366;
    public DateTime? StartDateUtc { get; set; } = DateTime.UtcNow;
    public DateTime? EndDateUtc{ get; set; }

}