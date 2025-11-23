using FitnessDuck.Data.Entities;
using FitnessDuck.Data.Repositories.Interfaces;
using FitnessDuck.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessDuck.Data.Repositories.Implementations;

public class LessonRepository : Repository<LessonEntity>, ILessonRepository
{
    public LessonRepository(FitnessDuckDbContext context) : base(context) { }

    public async Task<IEnumerable<LessonEntity>> GetUpcomingLessonsAsync(DateTime fromDate, DateTime toDate)
    {
        return await _dbSet
            .Include(l => l.Trainer)
                .Include(l=>l.Bookings)
                .ThenInclude(l => l.User)
                .Include(l=>l.Schedule)
            .Where(l => l.StartDateUtc >= fromDate &&  l.StartDateUtc <= toDate)
            .OrderBy(l => l.StartDateUtc)
            .ToListAsync();
    }

    public async Task<LessonEntity?> GetLessonWithBookingsAsync(Guid lessonId)
    {
        return await _dbSet
            .Include(l => l.Trainer)
                
            .Include(l => l.Bookings)
            .ThenInclude(b => b.User)
            .FirstOrDefaultAsync(l => l.Id == lessonId);
    }

    public async Task<bool> LessonExistsForSchedule(DateTime fromDate, Guid scheduleId)
    {
        return await _dbSet.AnyAsync(x=>x.ScheduleId == scheduleId && x.StartDateUtc >= fromDate);
    }

    public async Task<bool> isUserRegisteredtoLesson(Guid lessonId, Guid userId) => await _dbSet.AnyAsync(x=>x.Id==lessonId && x.Bookings.Any(b=>b.UserId == userId && b.Status != BookingStatus.Deleted));
    public async Task<LessonEntity> SubscribeToLesson(Guid lessonId, Guid userId, bool overbooking)
    {
       


        _context.Bookings.Add(new BookingEntity()
        {
            UserId = userId,
            Id = Guid.NewGuid(),
            LessonId = lessonId,
            BookingDateUtc =  DateTime.UtcNow,
            Status = overbooking ? BookingStatus.Pending : BookingStatus.Confirmed
        });

        await _context.SaveChangesAsync();
        
        

        var updated = await GetByIdAsync(lessonId);

        if (updated == null)
            throw new InvalidOperationException("Lesson not found.");

        return updated;
    }

    public async Task<LessonEntity> UnsubscribeFromLesson(Guid lessonId, Guid userId)
    {
       var toUpdate=await  _context.Bookings.FirstOrDefaultAsync(x=>x.LessonId == lessonId && x.UserId == userId && x.Status!=BookingStatus.Deleted);
       
       if (toUpdate == null)
           throw new InvalidOperationException("Booking not found.");
       
       toUpdate.Status = BookingStatus.Deleted;
       toUpdate.BookingDateUtc = DateTime.UtcNow;
       
        await _context.SaveChangesAsync();
        
        

        var updated = await GetByIdAsync(lessonId);

        if (updated == null)
            throw new InvalidOperationException("Lesson not found.");

        return updated;    }

    public async Task<IEnumerable<LessonEntity>> GetMyLessonsAsync(Guid userId) => await _dbSet
        .Include(x=>x.Schedule)
        .Include(x=>x.Trainer)
        .Where(x =>
        x.Bookings.Any(b => b.UserId == userId && b.Status != BookingStatus.Deleted)).ToListAsync();
}