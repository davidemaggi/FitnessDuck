using FitnessDuck.Data.Repositories.Interfaces;
using FitnessDuck.Models;
using FitnessDuck.Models.DTOs;
using FitnessDuck.Models.Entities;
using FitnessDuck.Notifications.Interfaces;
using MapsterMapper;

namespace FitnessDuck.Notifications.Implementations;

public class NotificationOutboxService:INotificationOutboxService
{
    private readonly IOutboxRepository _outboxRepository;
    private readonly IMapper _mapper;

    public NotificationOutboxService(IOutboxRepository outboxRepository, IMapper mapper)
    {
        _outboxRepository = outboxRepository;
        _mapper = mapper;
    }

    public async Task<NotificationsOutboxDto> AddMessage(NotificationsOutboxDto dto)
    {
        var ret=await _outboxRepository.AddMessage(
            new NotificationsOutboxEntity
            {
                Id = dto.Id ,
                UserId = dto.UserId,
                Method = dto.Method,
                Contact = dto.Contact,
                Message = dto.Message,
                Attempt = 0
            }
        );

        return _mapper.Map<NotificationsOutboxDto>(ret);


    }

    public async Task<IEnumerable<NotificationsOutboxDto>> GetPendingMessages()
    {
       
        return _mapper.Map<IEnumerable<NotificationsOutboxDto>>(await _outboxRepository.GetPendingMessages());

    }

    public async Task<bool> MessageSent(Guid msgId) => await _outboxRepository.MessageSent(msgId);


    public async Task  MessageFailed(Guid msgId) =>  await _outboxRepository.MessageSendFail(msgId);
    
}