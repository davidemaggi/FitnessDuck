namespace FitnessDuck.Data.Entities;

public class LessonEntity:BaseEntity
{
    public Guid? ScheduleId { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public int AvailableSeats { get; set; } = 5;
    public int Seats { get; set; }
    public string? Name { get; set; }
    
    public ICollection<BookingEntity> Bookings { get; set; } = new List<BookingEntity>();
    public ScheduleEntity? Schedule { get; set; }
}