using FitnessDuck.Auth.Interfaces;
using FitnessDuck.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessDuck.Api.Controllers;


[ApiController]
[Route("api/[controller]")]

public class TrainersController : Controller
{
    private readonly IUserService _userService;

    public TrainersController(IUserService userService
    )
    {
        _userService = userService;
     
    }
    
    [Authorize]
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<UserInfoDto>))]
    public async Task<IActionResult> GetTrainerList()
    {
        
      
        
        IEnumerable<UserInfoDto> trainersDto = await _userService.GetTrainers();

        //if (!lessonDtos.Any())
        //  return NotFound();
         
           

        
        
        return Ok(trainersDto);
    }
}