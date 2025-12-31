using FitnessDuck.Models;
using FitnessDuck.Models.Entities;

namespace FitnessDuck.Data.Entities;

public class UserEntity:BaseEntity
{
    public string Email { get; set; }
    public string? TelegramChatId { get; set; } = "";
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public UserRole Role { get; set; } = UserRole.Trainee;
    public bool EmailConfirmed { get; set; }
    public bool TelegramConfirmed { get; set; }
    public Guid? TelegramConfirmationKey { get; set; } = Guid.NewGuid();
    public ICollection<RefreshTokenEntity> RefreshTokens { get; set; } = new List<RefreshTokenEntity>();
    public ICollection<NotificationsOutboxEntity> PendingMessages { get; set; } = new List<NotificationsOutboxEntity>();

    public ICollection<BookingEntity> Bookings { get; set; } = new List<BookingEntity>();
    public ICollection<ScheduleEntity> Schedules { get; set; } = new List<ScheduleEntity>();
    public ICollection<LessonEntity> Lessons { get; set; } = new List<LessonEntity>();
    public string? PushSubscription { get; set; }
    public string Local { get; set; } = "it-IT";
    
    public UserPlan Plan { get; set; } = UserPlan.NoPlan;
    public int PlanAmount { get; set; } = 1;
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public DateTime? PlanExpiration { get; set; }
    
}