namespace FitnessDuck.Models.DTOs;

public class NotificationsOutboxDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string? Contact { get; set; } = null;
    
    public ContactMethod Method { get; set; }
    public string Message { get; set; }
    public UserDto? User { get; set; }
    public int Attempt { get; set; }
    public string? Subject { get; set; }
}