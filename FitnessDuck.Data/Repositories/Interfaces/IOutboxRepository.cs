using FitnessDuck.Data.Entities;
using FitnessDuck.Models.Entities;

namespace FitnessDuck.Data.Repositories.Interfaces;

public interface IOutboxRepository : IRepository<NotificationsOutboxEntity>
{
    Task<NotificationsOutboxEntity?> AddMessage(NotificationsOutboxEntity toAdd);
    Task<bool> MessageSent(Guid msgId);
    Task<NotificationsOutboxEntity?> MessageSendFail(Guid msgId);
    Task<IEnumerable<NotificationsOutboxEntity>> GetPendingMessages(); 
}