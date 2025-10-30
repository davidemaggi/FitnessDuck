namespace FitnessDuck.Data.Entities;

public class ScheduleEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "";
    public string TrainerId { get; set; } = "";
    public string Recurrence { get; set; } = ""; // daily/weekly
    public int Seats { get; set; }
    public int MinUnsubscribeMinutes { get; set; }
    public int AdvanceBookingMinutes { get; set; }
    public ICollection<LessonEntity> Lessons { get; set; } = new List<LessonEntity>();
}