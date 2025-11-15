namespace FitnessDuck.Models.DTOs;

public class BookingDto
{
    public Guid Id { get; set; }
    public Guid LessonId { get; set; }
    public Guid UserId { get; set; }
    public DateTime BookingDateUtc { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public UserDto User { get; set; } = default!;
    public LessonDto Lesson { get; set; } = default!;
}