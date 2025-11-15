using System.Security.Cryptography;
using FitnessDuck.Auth.Interfaces;
using FitnessDuck.Data.Repositories.Interfaces;

namespace FitnessDuck.Auth.Implementations;

public class TokenService : ITokenService
{
    private readonly ITokenRepository _tokenRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public TokenService(ITokenRepository tokenRepository, IRefreshTokenRepository refreshTokenRepository)
    {
        _tokenRepository = tokenRepository;
        _refreshTokenRepository = refreshTokenRepository;
    }

    

    public async Task<string> GenerateTokenAsync(string contact) => await _tokenRepository.GenerateTokenAsync(contact);

    public async Task<bool> ValidateTokenAsync(string contact, string token)=> await _tokenRepository.ValidateTokenAsync(contact, token);
}