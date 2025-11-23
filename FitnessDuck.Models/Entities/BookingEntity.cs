using FitnessDuck.Models;

namespace FitnessDuck.Data.Entities;

public class BookingEntity:BaseEntity
{
   
    public Guid LessonId { get; set; }
    public Guid UserId { get; set; }
    public DateTime BookingDateUtc { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Confirmed;
    public UserEntity User { get; set; } = default!;
    public LessonEntity Lesson { get; set; } = default!;
}
