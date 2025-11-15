using System.Security.Cryptography;
using FitnessDuck.Data.Entities;
using FitnessDuck.Data.Repositories.Interfaces;
using FitnessDuck.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FitnessDuck.Data.Repositories.Implementations;

public class RefreshTokenRepository: Repository<RefreshTokenEntity>, IRefreshTokenRepository
{
  


    public RefreshTokenRepository(FitnessDuckDbContext context) : base(context)
    {
    }




    public async Task<RefreshTokenEntity?> GetByTokenAsync(string token) => await _dbSet.FirstOrDefaultAsync(x=>x.Token == token);
    public async Task RevokeAsync(Guid oldTokenId)
    {
        var toRevoke = await GetByIdAsync(oldTokenId);
        
        
        toRevoke!.Revoked=DateTime.UtcNow;
        
        await Update(toRevoke);
        
        
    }
}