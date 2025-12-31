using FitnessDuck.Auth.Interfaces;
using FitnessDuck.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessDuck.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    
    
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    
    [Authorize(Roles =  "Admin")]
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<UserDto>))]
    public async Task<IActionResult> GetAllUsers()
    {
        
        IEnumerable<UserDto> usersDto = await _userService.GetAll();


        
        
        return Ok(usersDto);
    }
    
    [Authorize(Roles =  "Admin")]
    [HttpPost]
    [ProducesResponseType(200, Type = typeof(UserDto))]
    public async Task<IActionResult> SaveUser([FromBody] UserDto userDto)
    {
        
        UserDto usersDto = await _userService.SaveUserAsync(userDto);


        
        
        return Ok(usersDto);
    }
    
    
    
}