using FitnessDuck.Data.Entities;
using FitnessDuck.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessDuck.Data.Repositories.Implementations;

public class ScheduleRepository : Repository<ScheduleEntity>, IScheduleRepository
{
    public ScheduleRepository(FitnessDuckDbContext context) : base(context) { }

    
}