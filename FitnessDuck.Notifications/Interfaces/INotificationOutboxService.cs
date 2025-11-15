using FitnessDuck.Models.DTOs;

namespace FitnessDuck.Notifications.Interfaces;

public interface INotificationOutboxService
{
    public Task<NotificationsOutboxDto> AddMessage(NotificationsOutboxDto dto);
    Task<IEnumerable<NotificationsOutboxDto>> GetPendingMessages();
    Task<bool> MessageSent(Guid msgId);
    Task MessageFailed(Guid msgId);
}