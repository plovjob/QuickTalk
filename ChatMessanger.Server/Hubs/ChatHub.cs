using Microsoft.AspNetCore.SignalR;

namespace ChatMessanger.Server.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string text)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, text);
        }
    }
}
