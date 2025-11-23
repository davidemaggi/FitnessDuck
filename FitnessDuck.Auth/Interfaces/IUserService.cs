using FitnessDuck.Data.Entities;
using FitnessDuck.Models;
using FitnessDuck.Models.DTOs;

namespace FitnessDuck.Auth.Interfaces;

public interface IUserService
{
    Task<UserDto> CreateFirstUserAsync(string email, string? name, string? surname);
    Task<UserDto> CreateUserAsync(string email, string? name, string? surname, UserRole role);
    Task<UserDto> UpdateUserInfoAsync(UserInfoDto info);
    Task<UserDto?> GetByEmailAsync(string email);
    Task<UserDto?> GetByTelegramIdAsync(string telegramId);
    Task<UserDto> ConfirmContactMethod(ContactMethod contactMethod, UserDto user, string chatId="");
    Task<UserDto> GetByRefreshToken(string requestRefreshToken);
    Task<IEnumerable<UserInfoDto>> GetTrainers();
}
