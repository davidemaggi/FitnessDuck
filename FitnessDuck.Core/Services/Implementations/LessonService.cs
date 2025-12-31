using FitnessData.Core.Services.Interfaces;
using FitnessDuck.Data.Entities;
using FitnessDuck.Data.Repositories.Interfaces;
using FitnessDuck.Models;
using FitnessDuck.Models.DTOs;
using FitnessDuck.Notifications.Interfaces;
using MapsterMapper;

namespace FitnessDuck.Core.Services.Implementations;

public class LessonService:ILessonService
{
    private readonly ILessonRepository _lessonRepo;
    private readonly IScheduleService _scheduleService;
    private readonly IUserRepository _userRepo;
    private readonly IMapper _mapper;
    private readonly INotificationOutboxService _notification;

    public LessonService(ILessonRepository lessonRepo, IMapper mapper, IScheduleService scheduleService, IUserRepository userRepo, INotificationOutboxService notification)
    {
        _lessonRepo = lessonRepo;
        _mapper = mapper;
        _scheduleService = scheduleService;
        _userRepo = userRepo;
        _notification = notification;
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
            TrainerId =  lessonDto.TrainerId,
            Name = lessonDto.Name,
            Description = lessonDto.Description,
            AdvanceBookingDays = lessonDto.AdvanceBookingDays,
            MinUnsubscribeHours =  lessonDto.MinUnsubscribeHours,

        });
        
        
        return _mapper.Map<LessonDto>(added);

    }
    
    
    public async Task<IEnumerable<LessonDto>> GenerateLessonsFromSchedule(Guid scheduleId, bool deleteLessons=false)
    {
       var schedule = await _scheduleService.GetScheduleByIdAsync(scheduleId);

    var now = DateTime.Now;
    var endDate = now.AddDays(schedule.AdvanceBookingDays);
    var generatedLessons = new List<LessonDto>();

    // Collect all valid lesson start times from current schedule slots
    var validLessonStartTimes = new List<DateTime>();

    for (DateTime day = now.Date; day <= endDate.Date; day = day.AddDays(1))
    {
        var dayPlan = schedule.WeekPlan.FirstOrDefault(dp => dp.DayOfWeek == day.DayOfWeek);
        if (dayPlan == null || !dayPlan.IsActive)
            continue;

        foreach (var slot in dayPlan.Slots)
        {
            var lessonStart = new DateTime(day.Year, day.Month, day.Day, slot.Hour, slot.Minute, 0);
            if (lessonStart < now)
                continue;

            validLessonStartTimes.Add(lessonStart);

            bool exists = await LessonExistsForSchedule(lessonStart, schedule.Id);
            if (!exists)
            {
                generatedLessons.Add(new LessonDto
                {
                    Id = Guid.NewGuid(),
                    ScheduleId = schedule.Id,
                    StartDateUtc = lessonStart,
                    EndDateUtc = lessonStart.AddMinutes(schedule.DurationMinutes),
                    Seats = schedule.Seats,
                    Name = schedule.Name,
                    Description = schedule.Description,
                    TrainerId = schedule.TrainerId,
                    AdvanceBookingDays = schedule.AdvanceBookingDays,
                    MinUnsubscribeHours = schedule.MinUnsubscribeHours,
                });
            }
        }
    }

    // Fetch all existing lessons within the date range of schedule
    var tmpExistingLessons = await _lessonRepo.GetLessonsForScheduleBetweenDates(schedule.Id, now, endDate);
    var existingLessons=_mapper.Map<IEnumerable<LessonDto>>(tmpExistingLessons);
    // Find lessons to delete: those that are in existingLessons but NOT in validLessonStartTimes
    var lessonsToDelete = existingLessons
        .Where(lesson => !validLessonStartTimes.Contains(lesson.StartDateUtc))
        .ToList();

    // Delete lessons for removed slots
    if (deleteLessons)
    {
   

    foreach (var lesson in lessonsToDelete)
    {
        await _lessonRepo.Remove(lesson.Id);
        await _notification.LessonDeleted(lesson);

    }
    
    }
    else
    {
        foreach (var lesson in lessonsToDelete)
        {
            
            await _lessonRepo.RemoveLessonsFromScheduleAsync(lesson.Id);
            
        }
    }

    // Save new lessons
    foreach (var lesson in generatedLessons)
    {
        await SaveLessonAsync(lesson);
    }

    return generatedLessons;
    }

    
    
    
    
    public async Task<LessonDto> SubscribeToLesson(Guid lessonId, Guid userId)
    {
         var alreadyRegisterd = await _lessonRepo.isUserRegisteredtoLesson(lessonId, userId);
         
         if (alreadyRegisterd)
             throw new Exception("You cannot subscribe to this lesson");
         var lesson = await _lessonRepo.GetByIdAsync(lessonId);


         if (!await CanSubscribeToLesson(userId,lessonId))
         {
             throw new Exception("You cannot subscribe to this lesson because it exceeds your plan");
             
         }




         var overbooking = false;

         if (lesson != null)
         {
             overbooking = lesson.Seats<=lesson.Bookings.Count(b => b.Status==BookingStatus.Confirmed);
             
         }



         LessonEntity ret = await _lessonRepo.SubscribeToLesson(lessonId, userId,overbooking);

        
        return _mapper.Map<LessonDto>(ret);
    }

    public async Task<LessonDto> UnsubscribeFromLesson(Guid lessonId, Guid userId)
    {
        var alreadyRegisterd = await _lessonRepo.isUserRegisteredtoLesson(lessonId, userId);
         
        if (!alreadyRegisterd)
            throw new Exception("You cannot unsubscribe to this lesson");
         
        
        LessonEntity ret = await _lessonRepo.UnsubscribeFromLesson(lessonId, userId);

        
        return _mapper.Map<LessonDto>(ret);    }

    public async Task<IEnumerable<LessonDto>> GetMyLessonsAsync(Guid userId)
    {
        var lessons = await _lessonRepo.GetMyLessonsAsync(userId);
        
        
        
        return _mapper.Map<IEnumerable<LessonDto>>(lessons);    }
    
    
    public async Task<bool> CanSubscribeToLesson(Guid userId, Guid lessonId)
    {
        
        var user = await _userRepo.GetByIdAsync(userId);

        var lesson = await _lessonRepo.GetByIdAsync(lessonId);

        if (user.Role==UserRole.Trainee)
        {

            if (user.Plan == UserPlan.Weekly)
            {
            

            int diffToMonday = (7 + (lesson.StartDateUtc.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime weekStartUtc = lesson.StartDateUtc.Date.AddDays(-diffToMonday);
            DateTime weekEndUtc = weekStartUtc.AddDays(6).Date.AddDays(1).AddTicks(-1);
            
            
            var bookingsThisWeek = user.Bookings
                .Where(b => b.BookingDateUtc >= weekStartUtc && b.BookingDateUtc <= weekEndUtc && b.Status==BookingStatus.Confirmed)
                .ToList();

            return bookingsThisWeek.Count >= user.PlanAmount;


            }

            if (user.Plan == UserPlan.Weekly)
            {
                DateTime monthStartUtc = new DateTime(lesson.StartDateUtc.Year, lesson.StartDateUtc.Month, 1);
                DateTime monthEndUtc = monthStartUtc.AddMonths(1).AddTicks(-1);

                var bookingsThisMonth = user.Bookings
                    .Where(b => b.BookingDateUtc >= monthStartUtc && b.BookingDateUtc <= monthEndUtc && b.Status==BookingStatus.Confirmed)
                    .ToList();

            return bookingsThisMonth.Count >= user.PlanAmount;

            }
            
            if (user.Plan == UserPlan.NoPlan)
            {


                var nextBookings = user.Bookings
                    .Where(b => b.BookingDateUtc >= DateTime.UtcNow  && b.Status==BookingStatus.Confirmed)
                    .ToList();

                return nextBookings.Count >= user.PlanAmount;

            }

        }



        return true;
    }
    
    
    
}