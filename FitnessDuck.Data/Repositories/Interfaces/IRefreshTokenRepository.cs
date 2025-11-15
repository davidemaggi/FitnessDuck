using FitnessDuck.Data.Entities;
using FitnessDuck.Models.DTOs;

namespace FitnessDuck.Data.Repositories.Interfaces;

public interface IRefreshTokenRepository:IRepository<RefreshTokenEntity>
{
   
    Task<RefreshTokenEntity?> GetByTokenAsync(string token);
   // Task<RefreshTokenDto> RevokeToken(string token);
   Task RevokeAsync(Guid oldTokenId);
}