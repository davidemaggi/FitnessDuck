using FitnessDuck.Data.Entities;
using FitnessDuck.Models.Common;

namespace FitnessDuck.Models.DTOs;

public class ScheduleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public Guid? TrainerId { get; set; }
    public UserInfoDto? Trainer { get; set; }
    public RecurrenceType Recurrence { get; set; } =  RecurrenceType.None;
    public List<DayPlan> WeekPlan { get; set; } = DayPlan.BuildWeekPlan();

    public int DurationMinutes { get; set; } = 60;
    public int Seats { get; set; }
    public int MinUnsubscribeHours { get; set; } = 24;
    public int AdvanceBookingDays { get; set; } = 366;
    public DateTime StartDateUtc { get; set; }
    public DateTime? EndDateUtc{ get; set; }
    //public ICollection<LessonDto> Lessons { get; set; }
}