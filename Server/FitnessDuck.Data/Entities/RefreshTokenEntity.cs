namespace FitnessDuck.Data.Entities;

public class RefreshTokenEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; } = "";
    public DateTime Expiry { get; set; }
}