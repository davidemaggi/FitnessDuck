using FitnessData.Core.Services.Interfaces;
using FitnessDuck.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessDuck.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class SchedulesController : Controller
{

    private readonly  IScheduleService _scheduleService;
    private readonly  ILessonService _lessonService;
    
    
    public SchedulesController(IScheduleService scheduleService, ILessonService lessonService)
    {
        _scheduleService = scheduleService;
        _lessonService = lessonService;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ScheduleDto>))]
    public async Task<IActionResult> GetAllSchedules()
    {
        var scheduleDtos = await _scheduleService.GetAllSchedulesAsync();

       
        
        return Ok(scheduleDtos);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(500)]
    [ProducesResponseType(200, Type = typeof(ScheduleDto))]
    public async Task<IActionResult> SaveSchedule([FromBody] SaveScheduleDto payload)
    {
        
        
        ScheduleDto res = await _scheduleService.SaveScheduleAsync(payload);

        var ccc = await _lessonService.GenerateLessonsFromSchedule(res.Id, payload.DeleteLessons);
        
        
        
       
        return Ok(res);
    }
    
}