namespace FitnessDuck.Data.Repositories.Interfaces;

public interface ITokenRepository
{
    Task<string> GenerateTokenAsync(string contact);
    Task<bool> ValidateTokenAsync(string contact, string token);
}