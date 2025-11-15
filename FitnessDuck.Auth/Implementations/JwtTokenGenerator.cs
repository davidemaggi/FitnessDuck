using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FitnessDuck.Auth.Interfaces;
using FitnessDuck.Data.Entities;
using FitnessDuck.Data.Repositories.Interfaces;
using FitnessDuck.Models.DTOs;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FitnessDuck.Auth.Implementations;

public class JwtTokenGenerator: IJwtTokenGenerator
{
    private readonly IConfiguration _config;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IMapper _mapper;

    public JwtTokenGenerator(IConfiguration config, IRefreshTokenRepository refreshTokenRepository, IMapper mapper)
    {
        _config = config;
        _refreshTokenRepository = refreshTokenRepository;
        _mapper = mapper;
    }

    public string GenerateJwtToken(UserDto user)
    {
        var jwtSettings = _config.GetSection("Jwt");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            
            new Claim(ClaimTypes.Name, user.Name??""),
            new Claim(ClaimTypes.Surname, user.Surname??""),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Int32.Parse(jwtSettings["AccessTokenExpirationMinutes"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public RefreshTokenDto GenerateRefreshToken(UserDto user)
    {
        var jwtSettings = _config.GetSection("Jwt");
        
        var newToken= new RefreshTokenDto
        {
            Id = Guid.NewGuid(),
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            UserId = user.Id,
            Created = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddDays(Int32.Parse(jwtSettings["RefreshTokenExpirationDays"]))
        };

        _refreshTokenRepository.AddOrUpdateAsync(_mapper.Map<RefreshTokenEntity>(newToken));

       return newToken;

    }

    public Task RevokeTokenAsync(Guid oldTokenId) => _refreshTokenRepository.RevokeAsync(oldTokenId);
}