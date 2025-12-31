using FitnessDuck.Models.DTOs;

namespace FitnessData.Core.Services.Interfaces;

public interface ILessonService
{


    Task<IEnumerable<LessonDto>> GetUpcomingLessonsAsync(DateTime fromDate, DateTime toDate);
    Task<IEnumerable<LessonDto>> GenerateLessonsFromSchedule(Guid scheduleId, bool deleteLessons=false);
    Task<LessonDto> SubscribeToLesson(Guid lessonId, Guid userId);
    Task<LessonDto> UnsubscribeFromLesson(Guid lessonId, Guid userId);
    Task<IEnumerable<LessonDto>> GetMyLessonsAsync(Guid userId);
}