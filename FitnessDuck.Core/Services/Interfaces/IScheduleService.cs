using FitnessDuck.Models.DTOs;

namespace FitnessData.Core.Services.Interfaces;

public interface IScheduleService
{


    Task<IEnumerable<ScheduleDto>> GetAllSchedulesAsync();
    Task<ScheduleDto?> GetScheduleByIdAsync(Guid id);

    Task<ScheduleDto> SaveScheduleAsync(SaveScheduleDto payload);
}