namespace FitnessDuck.Models.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Local { get; set; } = "it-IT";
    public string Email { get; set; }
    public string? TelegramChatId { get; set; } = "";
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public UserRole Role { get; set; } = UserRole.Trainee;
    public bool EmailConfirmed { get; set; }
    public bool TelegramConfirmed { get; set; }
    public Guid? TelegramConfirmationKey { get; set; }
    public ICollection<RefreshTokenDto> RefreshTokens { get; set; }
    //public ICollection<BookingDto> Bookings { get; set; } = new List<BookingDto>();
    public string? PushSubscription { get; set; }
    public bool HasFullName()=>Name is not null && Surname is not null;

}