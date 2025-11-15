using System.Security.Cryptography;
using FitnessDuck.Data.Entities;
using FitnessDuck.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessDuck.Data.Repositories.Implementations;

public class TokenRepository: Repository<AuthTokenEntity>, ITokenRepository
{
    private readonly TimeSpan _tokenLifetime = TimeSpan.FromMinutes(15);


    public TokenRepository(FitnessDuckDbContext context) : base(context)
    {
    }

    private string GenerateRandomToken()
        => RandomNumberGenerator.GetInt32(100000, 999999).ToString(); // 6-digit OTP
    
    public async Task<string> GenerateTokenAsync(string contact)
    {
        var token = GenerateRandomToken();

        var authToken = new AuthTokenEntity()
        {
            Id = Guid.NewGuid(), Contact = contact, Token = token,
            Expiration = DateTime.UtcNow.Add(_tokenLifetime), Used = false
        };

        var newToken = await AddAsync(authToken, true);

        return newToken.Token;
    }

    public async Task<bool> ValidateTokenAsync(string contact, string token)
    {
        var authToken = await _dbSet
            .FirstOrDefaultAsync(t => t.Contact == contact && t.Token == token && !t.Used && t.Expiration > DateTime.UtcNow);
        if (authToken == null) return false;

        authToken.Used = true;
        var newToken = await Update(authToken, true);

        return true;
    }
}