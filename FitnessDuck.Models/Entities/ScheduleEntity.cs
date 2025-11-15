using FitnessDuck.Models;
using FitnessDuck.Models.Common;

namespace FitnessDuck.Data.Entities;

public class ScheduleEntity:BaseEntity
{
    public string Name { get; set; } = "";
    public string? Icon { get; set; }
    public Guid? TrainerId { get; set; }
    public RecurrenceType Recurrence { get; set; } = RecurrenceType.None;
    public List<DayPlan> WeekPlan { get; set; } = DayPlan.BuildWeekPlan();
    
    public int DurationMinutes { get; set; }
    public int Seats { get; set; }
    public int MinUnsubscribeHours { get; set; }
    public int AdvanceBookingDays{ get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime? EndDateUtc{ get; set; }
    public ICollection<LessonEntity> Lessons { get; set; } = new List<LessonEntity>();
}