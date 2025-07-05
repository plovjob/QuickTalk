using QuickTalk.Server.Models;

namespace QuickTalk.Server.Interfaces
{
    public interface IMessenger
    {
        Task SaveMessageAsync(Message message);
        Task<List<MessageDTO>> ShowAllMessages();
    }
}
