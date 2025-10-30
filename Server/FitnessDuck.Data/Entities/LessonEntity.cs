namespace FitnessDuck.Data.Entities;

public class LessonEntity
{
    public Guid Id { get; set; }
    public Guid ScheduleId { get; set; }
    public DateTime Date { get; set; }
    public int AvailableSeats { get; set; }
    public ICollection<BookingEntity> Bookings { get; set; } = new List<BookingEntity>();
    public ScheduleEntity Schedule { get; set; } = default!;
}