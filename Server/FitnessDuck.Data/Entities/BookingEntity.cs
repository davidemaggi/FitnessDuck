namespace FitnessDuck.Data.Entities;

public class BookingEntity
{
    public Guid Id { get; set; }
    public Guid LessonId { get; set; }
    public Guid UserId { get; set; }
    public DateTime BookingDate { get; set; }
    public string Status { get; set; } = ""; // Confirmed, Canceled, etc.
    public UserEntity User { get; set; } = default!;
    public LessonEntity Lesson { get; set; } = default!;
}
