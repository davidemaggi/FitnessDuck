namespace FitnessDuck.Models.DTOs;

public class ValidateTokenResponseDto
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public int ExpiresInSeconds { get; set; } = 300;



}
