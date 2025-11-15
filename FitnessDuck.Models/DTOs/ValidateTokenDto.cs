namespace FitnessDuck.Models.DTOs;

public class ValidateTokenDto
{
    public ContactMethod ContactMethod { get; set; } = ContactMethod.Email!;
    public string Contact { get; set; } = null!;
    public string Token { get; set; } = null!;
    public string? Name { get; set; } = null!; // For new user registrations
    public string? Surname { get; set; } = null!; // For new user registrations
}
