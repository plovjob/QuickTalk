using QuickTalk.Server.Models;

namespace QuickTalk.Server.Interfaces
{
    //должен быть написан в терминах модели бизнес логики
    public interface IMessenger
    {
        //SendMessageAsync
        Task SaveMessageAsync(Message message);
        //ListMessagesAsync
        //в интерфейсах лучше не использоваться мутабельные коллекции
        //list не должен возвращаться или приниматсья в интерфейс
        //если нам в интерфейсе нужна коллекция мы используем интерфейс
        //коллекции IReadonlyList<> (немутабельная коллекция)
        Task<List<MessageDTO>> ShowAllMessages();
    }
}
