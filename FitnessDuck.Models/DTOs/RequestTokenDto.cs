namespace FitnessDuck.Models.DTOs;

public class RequestTokenDto
{
    public string Contact { get; set; } = default!; // Email or TelegramId
    public ContactMethod ContactMethod { get; set; } = ContactMethod.Email!; 
}