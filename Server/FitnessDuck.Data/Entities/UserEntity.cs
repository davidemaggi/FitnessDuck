namespace FitnessDuck.Data.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    public string Email { get; set; } = "";
    public string TelegramChatId { get; set; } = "";
    public string Name { get; set; } = "";
    public string Surname { get; set; } = "";
    public string Role { get; set; } = ""; // Admin, Trainer, Trainee
    public ICollection<BookingEntity> Bookings { get; set; } = new List<BookingEntity>();
    public string? PushSubscription { get; set; }
}