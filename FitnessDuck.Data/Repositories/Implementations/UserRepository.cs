using FitnessDuck.Data.Entities;
using FitnessDuck.Data.Repositories.Interfaces;
using FitnessDuck.Models;
using FitnessDuck.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FitnessDuck.Data.Repositories.Implementations;

public class UserRepository : Repository<UserEntity>, IUserRepository
{
    public UserRepository(FitnessDuckDbContext context) : base(context)
    {
    }
    
    public async Task<UserEntity> CreateFirstUserAsync(string email, string? name, string? surname)
    {
        var existingUsers = await _dbSet.AnyAsync();
        if (existingUsers)
            throw new InvalidOperationException("First user already exists");

        return await CreateUserAsync(email, name, surname, UserRole.Admin);
    }

    public async Task<UserEntity> CreateUserAsync(string email, string? name, string? surname, UserRole role)
    {
        var existingUsers = await _dbSet.AnyAsync();
        if (existingUsers)
            throw new InvalidOperationException("User already exists");
        
        var user = new UserEntity() { Id = Guid.NewGuid(), Email = email, Name = name,Surname = surname, Role = UserRole.Admin };
        await AddAsync(user,true);
        return user;
    }

    public async Task<UserEntity> CreateUserAsync(UserDto dto)
    {
        var user = new UserEntity();
        
        var existingUser = await _dbSet.FindAsync(dto.Id);
        if (existingUser is not null)
        {
            user = existingUser;
            
            user.Name = dto.Name;
            user.Surname = dto.Surname;
            user.PlanExpiration = dto.PlanExpiration;
            user.PlanAmount = dto.PlanAmount;
            user.PlanAmount = dto.PlanAmount;
        }
        else
        {
            user = new UserEntity
            {
                Id = dto.Id,
                Email = dto.Email,
                Name = dto.Name,
                Surname = dto.Surname,
                Role = dto.Role,
                EmailConfirmed = dto.EmailConfirmed,
           
            
           
                Plan = dto.Plan,
                PlanAmount = dto.PlanAmount,
                RegistrationDate = DateTime.UtcNow,
                PlanExpiration = dto.PlanExpiration
            };
        }

        
        
        await AddOrUpdateAsync(user,true);
        return user;
    }

    public async Task<UserEntity> UpdateUserInfoAsync(UserInfoDto info)
    {
  
        
        var existingUser = await GetByIdAsync(info.Id);
        if (existingUser is null)
            throw new InvalidOperationException("User not found");

        existingUser.Name = info.Name;
        existingUser.Surname = info.Surname;
        
        var updated= await Update(existingUser,true);
        return updated;
    }

    public async Task<UserEntity?> GetByRefreshToken(string refreshToken) => await _dbSet.Include(u=>u.RefreshTokens).FirstOrDefaultAsync(u=>u.RefreshTokens.Any(r=>r.Token == refreshToken));
    public async Task<IEnumerable<UserEntity>> GetTrainers() => await _dbSet.Where(u => u.Role == UserRole.Trainer || u.Role == UserRole.Admin).ToListAsync();

    public async Task<UserEntity?>GetByEmailAsync(string email) {return await _dbSet.Include(u=>u.RefreshTokens).FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());} 

    public async Task<UserEntity?>  GetByTelegramIdAsync(string telegramChatId)=> await _dbSet.Include(u=>u.RefreshTokens).FirstOrDefaultAsync(u =>u.TelegramChatId.ToLower() == telegramChatId.ToLower() || u.TelegramConfirmationKey.ToString().ToLower() == telegramChatId.ToLower());
}