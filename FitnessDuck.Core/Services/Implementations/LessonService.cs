using FitnessData.Core.Services.Interfaces;
using FitnessDuck.Data.Entities;
using FitnessDuck.Data.Repositories.Interfaces;
using FitnessDuck.Models.DTOs;
using MapsterMapper;

namespace FitnessDuck.Core.Services.Implementations;

public class LessonService:ILessonService
{
    private readonly ILessonRepository _lessonRepo;
    private readonly IScheduleService _scheduleService;
    private readonly IMapper _mapper;

    public LessonService(ILessonRepository lessonRepo, IMapper mapper, IScheduleService scheduleService)
    {
        _lessonRepo = lessonRepo;
        _mapper = mapper;
        _scheduleService = scheduleService;
    }

    public async Task<IEnumerable<LessonDto>> GetUpcomingLessonsAsync(DateTime fromDate, DateTime toDate)
    {
        var lessons = await _lessonRepo.GetUpcomingLessonsAsync(fromDate, toDate);
        
        
        
        return _mapper.Map<IEnumerable<LessonDto>>(lessons);
    }
    
    
    public async Task<bool> LessonExistsForSchedule(DateTime fromDate, Guid scheduleId)
    {
        return await _lessonRepo.LessonExistsForSchedule(fromDate, scheduleId);

    }
    
    
    public async Task<LessonDto> SaveLessonAsync(LessonDto lessonDto)
    {
        var added= await _lessonRepo.AddOrUpdateAsync(new LessonEntity
        {
            Id = lessonDto.Id,
            ScheduleId = lessonDto.ScheduleId,
            StartDateUtc =lessonDto.StartDateUtc,
            EndDateUtc =lessonDto.EndDateUtc,
            Seats = lessonDto.Seats,

        });
        
        
        return _mapper.Map<LessonDto>(added);

    }
    
    
    public async Task<IEnumerable<LessonDto>> GenerateLessonsFromSchedule(Guid scheduleId)
    {
        var schedule = await _scheduleService.GetScheduleByIdAsync(scheduleId);
        
        
        var now = DateTime.Now;
        var endDate = now.AddDays(21);
        var generatedLessons = new List<LessonDto>();

        for (DateTime day = now.Date; day <= endDate.Date; day = day.AddDays(1))
        {
            // Find the DayPlan for the current day
            var dayPlan = schedule.WeekPlan.FirstOrDefault(dp => dp.DayOfWeek == day.DayOfWeek);

            if (dayPlan == null || !dayPlan.IsActive)
                continue; // Skip inactive or missing day plans

            foreach (var slot in dayPlan.Slots)
            {
                // Compose lesson start datetime
                var lessonStart = new DateTime(day.Year, day.Month, day.Day, slot.Hour, slot.Minute, 0);

                // Only generate lessons in the future (or now)
                if (lessonStart < now)
                    continue;

                // Check if a lesson at that exact time already exists
                bool exists = await LessonExistsForSchedule(lessonStart, schedule.Id);

                if (!exists)
                {
                    // Create new lesson and add to list
                    generatedLessons.Add(new LessonDto
                    {
                        Id = Guid.NewGuid(),
                        ScheduleId = schedule.Id,
                        StartDateUtc = lessonStart,
                        EndDateUtc = lessonStart.AddMinutes(schedule.DurationMinutes),
                        Seats = schedule.Seats

                    });
                }
            }
        }


        foreach (var lesson in generatedLessons)
        {
            await SaveLessonAsync(lesson);
        }








        return  generatedLessons;
    }

    public async Task<LessonDto> SubscribeToLesson(Guid lessonId, Guid userId)
    {
         var alreadyRegisterd = await _lessonRepo.isUserRegisteredtoLesson(lessonId, userId);
         
         if (alreadyRegisterd)
             throw new Exception("You cannot subscribe to this lesson");
         
        
         LessonEntity ret = await _lessonRepo.SubscribeToLesson(lessonId, userId);

        
        return _mapper.Map<LessonDto>(ret);
    }

    public async Task<LessonDto> UnsubscribeFromLesson(Guid lessonId, Guid userId)
    {
        var alreadyRegisterd = await _lessonRepo.isUserRegisteredtoLesson(lessonId, userId);
         
        if (!alreadyRegisterd)
            throw new Exception("You cannot unsubscribe to this lesson");
         
        
        LessonEntity ret = await _lessonRepo.UnsubscribeFromLesson(lessonId, userId);

        
        return _mapper.Map<LessonDto>(ret);    }
}