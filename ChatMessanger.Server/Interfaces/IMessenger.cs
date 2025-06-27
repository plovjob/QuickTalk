using ChatMessanger.Server.Models;

namespace ChatMessanger.Server.Interfaces
{
    public interface IMessenger
    {
        Task SaveMessageAsync(Message message);
        Task<List<MessageDTO>> ShowAllMessages();
    }
}
