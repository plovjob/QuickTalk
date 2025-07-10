using Microsoft.AspNetCore.SignalR;

namespace QuickTalk.Messages.WebApi.Hubs;

public class MessageHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        //после получения данных с клиента рассылает данные всем получателям
        await Clients.All.SendAsync("ReceiveMessage",user, message);
    }
}
