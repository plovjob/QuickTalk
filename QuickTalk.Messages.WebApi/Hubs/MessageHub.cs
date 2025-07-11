using Microsoft.AspNetCore.SignalR;

namespace QuickTalk.Messages.WebApi.Hubs;

internal sealed class MessageHub : Hub
{
    public async Task SendMessageAsync(string user, string message) =>
        await Clients.All.SendAsync("ReceiveMessage", user, message);
}
