using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FitnessData.Core.Services.Interfaces;
using FitnessDuck.Auth.Interfaces;
using FitnessDuck.Mail.Interfaces;
using FitnessDuck.Models;
using FitnessDuck.Models.DTOs;
using FitnessDuck.Notifications.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FitnessDuck.Api.Controllers;


[ApiController]
[Route("api/[controller]")]

public class AuthController : Controller
{

    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly IJwtTokenGenerator _jwtGenerator;
    private readonly INotificationOutboxService _outboxService;
    //private readonly IEmailSender _emailSender;
    private readonly IConfiguration _config;
    //private readonly ITelegramBotSender _telegramSender;
    
    public AuthController(IUserService userService, ITokenService tokenService, IJwtTokenGenerator jwtGenerator
        //,IEmailSender emailSender
        , IConfiguration config, INotificationOutboxService outboxService
        //, ITelegramBotSender telegramSender
        )
    {
        _userService = userService;
        _tokenService = tokenService;
        _jwtGenerator = jwtGenerator;
       // _emailSender = emailSender;
        _config = config;
        _outboxService = outboxService;
        //_telegramSender = telegramSender;
    }
    
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        

        

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);
        var role = User.FindFirstValue(ClaimTypes.Role);
        // return user info or call your service with userId
        return  Ok(await _userService.GetByEmailAsync(email));
    }
    

    [HttpPost("request-token")]
    public async Task<IActionResult> RequestToken([FromBody] RequestTokenDto dto)
    {
        var token = await _tokenService.GenerateTokenAsync(dto.Contact);

        var user = await _userService.GetByEmailAsync(dto.Contact);
        

        switch (dto.ContactMethod)
        {
            case ContactMethod.Email:
                //_emailSender.SendTokenAsync(dto.Contact, token);
                await _outboxService.AddMessage(new NotificationsOutboxDto
                {
                    Id = Guid.NewGuid(),
                    Method = ContactMethod.Email,
                    Message = string.Format("Your token is <b>{0}</b> ",token),
                    Contact = dto.Contact,
                    Attempt = 0
                });

                if (user is not null && user.TelegramConfirmed)
                {
                    
                    await _outboxService.AddMessage(new NotificationsOutboxDto
                    {
                        Id = Guid.NewGuid(),
                        Method = ContactMethod.Telegram,
                        Message = string.Format("Your token is {0}",token),
                        Contact = user.TelegramChatId,
                        Attempt = 0
                    });
                    
                }


                break;
            case ContactMethod.Telegram:
                
               await _outboxService.AddMessage(new NotificationsOutboxDto
                {
                    Id = Guid.NewGuid(),
                    Method = ContactMethod.Telegram,
                    Message = string.Format("Your token is {0}",token),
                    Contact = dto.Contact,
                    Attempt = 0
                });
                break;
            default:
                break;
                
        }
           // await _telegramSender.SendTokenAsync(dto.Contact, token);
        
        return Ok(token);
    }

    [HttpPost("validate-token")]
    public async Task<IActionResult> ValidateToken([FromBody] ValidateTokenDto dto)
    {
        var valid = await _tokenService.ValidateTokenAsync(dto.Contact, dto.Token);
        if (!valid) return Unauthorized("Invalid token.");

        var user = dto.ContactMethod==ContactMethod.Email
            ? await _userService.GetByEmailAsync(dto.Contact)
            : await _userService.GetByTelegramIdAsync(dto.Contact);

        if (user == null)
        {
            var isFirst = !(await _userService.GetByEmailAsync(dto.Contact) is not null);
            user = isFirst
                ? await _userService.CreateFirstUserAsync(dto.Contact, dto.Name, dto.Surname)
                : await _userService.CreateUserAsync(dto.Contact, dto.Name, dto.Surname, UserRole.Trainee);
        }

        await _userService.ConfirmContactMethod(dto.ContactMethod, user);
        
        
        var newAccessToken = _jwtGenerator.GenerateJwtToken(user);
        var newRefreshToken = _jwtGenerator.GenerateRefreshToken(user);
        
        var jwtSettings = _config.GetSection("Jwt");

        ;
        
        return Ok(new ValidateTokenResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token,
            ExpiresInSeconds = int.Parse(jwtSettings["AccessTokenExpirationMinutes"])*60
        });
    }

    [HttpPost("validate-token-only")]
    public async Task<IActionResult> ValidateTokenOnly([FromBody] ValidateTokenDto dto)
    {
        var valid = await _tokenService.ValidateTokenAsync(dto.Contact, dto.Token);
        if (!valid) return Unauthorized("Invalid token.");

        

        
        return Ok();
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] TokenRefreshRequestDto request)
    {

        
        var user= await _userService.GetByRefreshToken(request.RefreshToken);

        if (user is null)
            return Unauthorized();
        
        var oldToken = user.RefreshTokens.FirstOrDefault(x=>x.Token==request.RefreshToken);

        if (oldToken == null || !oldToken.IsActive)
            return Unauthorized();


        await _jwtGenerator.RevokeTokenAsync(oldToken.Id);
        var newAccessToken = _jwtGenerator.GenerateJwtToken(user!);
        var newRefreshToken = _jwtGenerator.GenerateRefreshToken(user!);
        return Ok(new
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token
        });
    }
    
    
    
    
    
    [Authorize]
    [HttpPost("update-user-info")]
    public async Task<IActionResult> UpdateUserInfo([FromBody] UserInfoDto dto)
    {
        
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        dto.Id = Guid.Parse(userId!);
        var uodated = await _userService.UpdateUserInfoAsync(dto);
        return Ok(uodated);
    }
    
    
    // Optional Admin-only: Create Trainer
    [Authorize(Roles = "Admin")]
    [HttpPost("create-trainer")]
    public async Task<IActionResult> CreateTrainer([FromBody] CreateUserDto dto)
    {
        var trainer = await _userService.CreateUserAsync(dto.Email, dto.Name, dto.Surname,UserRole.Trainer);
        return Ok(trainer);
    }
    
    
}