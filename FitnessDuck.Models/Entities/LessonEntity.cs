namespace FitnessDuck.Data.Entities;

public class LessonEntity:BaseEntity
{
    public Guid? ScheduleId { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public int Seats { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public Guid? TrainerId { get; set; }
    public bool Manual { get; set; } = false;

    public int MinUnsubscribeHours { get; set; } = 24;
    public int AdvanceBookingDays { get; set; } = 90;
    
    public ICollection<BookingEntity> Bookings { get; set; } = new List<BookingEntity>();
    public UserEntity? Trainer { get; set; }
    public ScheduleEntity? Schedule { get; set; }
}