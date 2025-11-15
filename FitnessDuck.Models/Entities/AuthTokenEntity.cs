namespace FitnessDuck.Data.Entities;

public class AuthTokenEntity : BaseEntity
{
    
    public string Contact { get; set; } = null!; // Email or Telegram ChatId
    public string Token { get; set; } = null!;
    public DateTime Expiration { get; set; }
    public bool Used { get; set; }
}