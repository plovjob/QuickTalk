using QuickTalk.Messages.Domain.Entities;

namespace QuickTalk.Messages.Domain.Interfaces;

public interface IMessageRepository
{
    Task SendMessageAsync(Message message);
    Task<IEnumerable<Message>> GetAllMessagesAsync();
}
