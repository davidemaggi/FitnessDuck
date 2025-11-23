using System.Security.Claims;
using FitnessData.Core.Services.Interfaces;
using FitnessDuck.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessDuck.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class LessonsController : Controller
{

    private readonly  ILessonService _lessonService;
    
    
    public LessonsController(ILessonService lessonService)
    {
        _lessonService = lessonService;
    }

    [HttpGet("upcoming")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<LessonDto>))]
    public async Task<IActionResult> GetUpcomingLessons([FromQuery] string? fromStr, [FromQuery] string? toStr)
    {
        
       DateTime from= fromStr is null ? DateTime.UtcNow: DateTime.Parse(fromStr);
       DateTime to= toStr is null ? DateTime.UtcNow.AddDays(40): DateTime.Parse(toStr);
       
       
       var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var lessonDtos = await _lessonService.GetUpcomingLessonsAsync((DateTime)from, (DateTime)to);

        //if (!lessonDtos.Any())
          //  return NotFound();
         
           

        
        
        return Ok(lessonDtos);
    }
    
    
    [Authorize]
    [HttpGet("mySubscriptions")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<LessonDto>))]
    public async Task<IActionResult> GetMySubscriptions()
    {
        IEnumerable<LessonDto> ret= new List<LessonDto>();

       
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        
        if (userId!=null)
            ret = await _lessonService.GetMyLessonsAsync(Guid.Parse(userId));

    
         
           

        
        
        return Ok(ret);
    }
    
    [Authorize]
    [HttpPut("subscribe")]
    [ProducesResponseType(500)]
    [ProducesResponseType(200, Type = typeof(LessonDto))]
    public async Task<IActionResult> Subscribe([FromQuery] Guid lessonId, [FromQuery] Guid userId)
    {
        
        var myUserId= User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId!=Guid.Parse(myUserId!))
        {
            return Unauthorized();
        }

        LessonDto ret = await _lessonService.SubscribeToLesson(lessonId, userId);
        
        
        
       
        return Ok(ret);
    }
    
    [Authorize]
    [HttpDelete("subscribe")]
    [ProducesResponseType(500)]
    [ProducesResponseType(200, Type = typeof(LessonDto))]
    public async Task<IActionResult> Unsubscribe([FromQuery] Guid lessonId, [FromQuery] Guid userId)
    {
        
        var myUserId= User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId!=Guid.Parse(myUserId!))
        {
            return Unauthorized();
        }

        LessonDto ret = await _lessonService.UnsubscribeFromLesson(lessonId, userId);
        
        
        
       
        return Ok(ret);
    }
}