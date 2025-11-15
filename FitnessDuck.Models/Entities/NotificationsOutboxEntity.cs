using FitnessDuck.Data.Entities;

namespace FitnessDuck.Models.Entities;

public class NotificationsOutboxEntity:BaseEntity
{
   
    public Guid? UserId { get; set; }
    public string? Contact { get; set; } = null;
    
    public ContactMethod Method { get; set; }
    public string Message { get; set; }
    public UserEntity? User { get; set; } = default!;
    public int Attempt { get; set; } = 0;

}
