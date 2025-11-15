namespace FitnessDuck.Data.Entities;

public class RefreshTokenEntity:BaseEntity
{

    public string Token { get; set; } = null!;
    public Guid UserId { get; set; }
    public DateTime Expires { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Revoked { get; set; }
    public bool IsActive => Revoked == null && !IsExpired;
    public bool IsExpired => DateTime.UtcNow > Expires;
    
    public UserEntity User { get; set; } = default!;
    
}