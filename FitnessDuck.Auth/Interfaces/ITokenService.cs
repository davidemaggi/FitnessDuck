namespace FitnessDuck.Auth.Interfaces;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(string contact);
    Task<bool> ValidateTokenAsync(string contact, string token);
}