namespace FitnessDuck.Models.DTOs;

public class RefreshTokenDto
{
    public Guid Id { get; set; }
    public string Token { get; set; } = null!;
    public Guid UserId { get; set; }
    public DateTime Expires { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Revoked { get; set; }
    public bool IsActive { get; set; }
    public bool IsExpired { get; set; }
}