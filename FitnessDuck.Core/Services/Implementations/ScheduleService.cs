using FitnessData.Core.Services.Interfaces;
using FitnessDuck.Data.Entities;
using FitnessDuck.Data.Repositories.Interfaces;
using FitnessDuck.Exceptions;
using FitnessDuck.Models;
using FitnessDuck.Models.DTOs;
using MapsterMapper;

namespace FitnessDuck.Core.Services.Implementations;

public class ScheduleService : IScheduleService
{
    private readonly IScheduleRepository _scheduleRepo;
    private readonly IMapper _mapper;

    public ScheduleService(IScheduleRepository scheduleRepo, IMapper mapper)
    {
        _scheduleRepo = scheduleRepo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ScheduleDto>> GetAllSchedulesAsync()
    {
        var schedules = await _scheduleRepo.GetAllAsync();
        return _mapper.Map<IEnumerable<ScheduleDto>>(schedules);
    }

    public async Task<ScheduleDto?> GetScheduleByIdAsync(Guid id)
    {
        var schedule= await _scheduleRepo.GetByIdAsync(id);


        return _mapper.Map<ScheduleDto>(schedule);
}

public async Task<ScheduleDto> SaveScheduleAsync(SaveScheduleDto payload)
    {

        ValidateSchedulePayload(payload);

        var toAdd = new ScheduleEntity
        {

            Id = payload.Id ?? Guid.NewGuid(),
            Name = payload.Name,
            Icon = payload.Icon,
            TrainerId = payload.TrainerId,
            Recurrence = payload.Recurrence,
            WeekPlan = payload.WeekPlan,
            Seats = payload.Seats,
            DurationMinutes = payload.DurationMinutes,
            MinUnsubscribeHours = payload.MinUnsubscribeHours,
            AdvanceBookingDays = payload.AdvanceBookingDays,
            StartDateUtc = payload.StartDateUtc ?? DateTime.UtcNow,
            EndDateUtc = payload.EndDateUtc,
            

        };
        
        var res = await _scheduleRepo.AddOrUpdateAsync(toAdd,true);

        if (res is null)
        {
            throw new FitnessDuckServerException(ErrorCodes.GENERIC_ERROR, "Could not save schedule");
        }


        return _mapper.Map<ScheduleDto>(res);
    }

    private void ValidateSchedulePayload(SaveScheduleDto payload)
    {

        if (payload.Recurrence==RecurrenceType.WeekPlan && payload.WeekPlan.Count != 7)
        {
            throw new FitnessDuckServerException(ErrorCodes.SCHEDULE_WEEK_PLAN_SIZE, "Week Plan must have 7 Days");
            
        }


    }
    
}