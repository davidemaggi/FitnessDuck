using FitnessDuck.Auth.Interfaces;
using FitnessDuck.Data.Entities;
using FitnessDuck.Data.Repositories.Interfaces;
using FitnessDuck.Exceptions;
using FitnessDuck.Models;
using FitnessDuck.Models.DTOs;
using MapsterMapper;

namespace FitnessDuck.Auth.Implementations;

public class UserService : IUserService
{
 
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;


    public UserService(IMapper mapper, IUserRepository userRepository)
    {
        _mapper = mapper;
        _userRepository = userRepository;
    }

    public async Task<UserDto> CreateFirstUserAsync(string email, string? name,string? surname)
    {
        var newuser = await _userRepository.CreateFirstUserAsync(email, name, surname);
        return _mapper.Map<UserDto>(newuser);
        
    }

    public async Task<UserDto> CreateUserAsync(string email, string? name,string? surname, UserRole role)
    {
        var newuser = await _userRepository.CreateUserAsync(email, name, surname, UserRole.Trainee);
        return _mapper.Map<UserDto>(newuser);
    }
    
    public async Task<UserDto> UpdateUserInfoAsync(UserInfoDto info)
    {
        
        var updatedUser = await _userRepository.UpdateUserInfoAsync(info);
        return _mapper.Map<UserDto>(updatedUser);
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        
        var user = await _userRepository.GetByEmailAsync(email);

        if (user is null)
            return null;
        
        return _mapper.Map<UserDto>(user);
    }


    public async Task<UserDto?> GetByTelegramIdAsync(string chatId)
    {
        
        var user = await _userRepository.GetByTelegramIdAsync(chatId);
        
        if (user is null)
            return null;

        
        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> ConfirmContactMethod(ContactMethod contactMethod, UserDto user, string chatId = "")
    {
        var toConfirm= await _userRepository.GetByIdAsync(user.Id);
        
        if (toConfirm is null) 
            throw new FitnessDuckServerException("error_user_not_found","User not found");

        if (contactMethod==ContactMethod.Email && !user.EmailConfirmed)
            toConfirm.EmailConfirmed = true;

        if (contactMethod == ContactMethod.Telegram && !user.TelegramConfirmed)
        {
        toConfirm.TelegramConfirmed = true;
        toConfirm.TelegramChatId = chatId;
        toConfirm.TelegramConfirmationKey = null;
        }
        

       var ret= await _userRepository.Update(toConfirm);
        
        return _mapper.Map<UserDto>(ret);
    }

    public async Task<UserDto> GetByRefreshToken(string refreshToken)
    {
        var user = await _userRepository.GetByRefreshToken(refreshToken);
        
        if (user is null)
            return null;

        
        return _mapper.Map<UserDto>(user);    }

    public async Task<IEnumerable<UserInfoDto>> GetTrainers()
    {
        IEnumerable<UserEntity> trainers = await _userRepository.GetTrainers();
        
       

        
        return _mapper.Map<IEnumerable<UserInfoDto>>(trainers); 
    }
}