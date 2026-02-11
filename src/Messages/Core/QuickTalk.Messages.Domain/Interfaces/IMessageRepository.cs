using QuickTalk.Messages.Domain.Entities;

namespace QuickTalk.Messages.Domain.Interfaces;

public interface IMessageRepository
{
    Task<OperationResult> CreateNewUserAsync(MessengerUser user);
    Task<OperationResult> SaveUserMessageAsync(Message message);
    Task<OperationResult<IReadOnlyCollection<Message>>> GetUserMessagesAsync(Guid senderId, Guid consumerId);
    Task<OperationResult<IReadOnlyCollection<ChatPartner>>> GetUserChatPartnersAsync(Guid consumerId);
}
