using FitnessDuck.Data.Entities;
using FitnessDuck.Models.DTOs;

namespace FitnessDuck.Auth.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateJwtToken(UserDto user);
    RefreshTokenDto GenerateRefreshToken(UserDto user);

    Task RevokeTokenAsync(Guid oldTokenId);
}