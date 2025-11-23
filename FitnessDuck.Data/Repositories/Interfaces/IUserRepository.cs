using FitnessDuck.Data.Entities;
using FitnessDuck.Models;
using FitnessDuck.Models.DTOs;

namespace FitnessDuck.Data.Repositories.Interfaces;

public interface IUserRepository: IRepository<UserEntity>
{
    public Task<UserEntity> CreateFirstUserAsync(string email, string? name, string? surname);
    public Task<UserEntity> CreateUserAsync(string email, string? name, string? surname, UserRole role);
    public Task<UserEntity> UpdateUserInfoAsync(UserInfoDto info);
    
    public Task<UserEntity?> GetByTelegramIdAsync(string email);
    public Task<UserEntity?> GetByEmailAsync(string telegramChatId);


    Task<UserEntity?> GetByRefreshToken(string refreshToken);
    Task<IEnumerable<UserEntity>> GetTrainers();
} 