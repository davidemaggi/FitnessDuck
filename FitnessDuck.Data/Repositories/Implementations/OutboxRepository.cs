using FitnessDuck.Data.Entities;
using FitnessDuck.Data.Repositories.Interfaces;
using FitnessDuck.Exceptions;
using FitnessDuck.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FitnessDuck.Data.Repositories.Implementations;

public class OutboxRepository : Repository<NotificationsOutboxEntity>, IOutboxRepository
{
    private readonly FitnessDuckDbContext _context;
    public OutboxRepository(FitnessDuckDbContext context) : base(context)
    {
        _context = context;
    }

    
    
    
    

    public async Task<NotificationsOutboxEntity?> AddMessage(NotificationsOutboxEntity toAdd) => await AddOrUpdateAsync(toAdd);

    public async Task<bool> MessageSent(Guid msgId)
    {
        var toRemove = await GetByIdAsync(msgId);

        if (toRemove == null)
            throw new InvalidOperationException("Message not found");

        await Remove(toRemove);
        return true;
    }

    public async Task<NotificationsOutboxEntity?> MessageSendFail(Guid msgId)
    {
        var failed = await GetByIdAsync(msgId);

        if (failed == null)
            throw new InvalidOperationException("Message not found");

        failed.Attempt += 1;
        
        return await AddOrUpdateAsync(failed);
        
        
    }

    public async Task<IEnumerable<NotificationsOutboxEntity>> GetPendingMessages()
    {
        var ret = await _context.Outbox.ToListAsync();


        return ret;
    }
}