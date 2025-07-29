using QuickTalk.Messages.Domain.Dto;
using QuickTalk.Messages.Domain.Entities;

namespace QuickTalk.Messages.Domain.Interfaces;

public interface IMessageRepository
{
    Task<OperationResult<MessageDto>> SendMessageAsync(MessageDto message);
    Task<OperationResult<IReadOnlyCollection<MessageDto>>> GetAllMessagesAsync();
}
