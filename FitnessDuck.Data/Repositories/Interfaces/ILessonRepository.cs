using FitnessDuck.Data.Entities;

namespace FitnessDuck.Data.Repositories.Interfaces;

public interface ILessonRepository : IRepository<LessonEntity>
{
    Task<IEnumerable<LessonEntity>> GetUpcomingLessonsAsync(DateTime fromDate, DateTime toDate);
    Task<LessonEntity?> GetLessonWithBookingsAsync(Guid lessonId);
    Task<bool> LessonExistsForSchedule(DateTime fromDate, Guid scheduleId);
    Task<bool> isUserRegisteredtoLesson(Guid lessonId, Guid userId);
    Task<LessonEntity> SubscribeToLesson(Guid lessonId, Guid userId, bool overbooking);
    Task<LessonEntity> UnsubscribeFromLesson(Guid lessonId, Guid userId);
    Task<IEnumerable<LessonEntity>> GetMyLessonsAsync(Guid userId);
}