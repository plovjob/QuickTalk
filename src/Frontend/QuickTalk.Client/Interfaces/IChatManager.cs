using QuickTalk.Client.Models;

namespace QuickTalk.Client.Interfaces;

internal interface IChatManager
{
    Task SendMessageAsync(MessageDto content);
    Task<List<MessageDto>?> GetMessagesAsync(Guid senderId, Guid consumerId);
}
