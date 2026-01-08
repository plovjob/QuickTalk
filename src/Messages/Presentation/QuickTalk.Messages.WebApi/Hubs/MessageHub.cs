using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace QuickTalk.Messages.WebApi.Hubs;

[Authorize]
public sealed class MessageHub : Hub
{
    public event Func<Task>? NotifyHelloMessage;

    public async Task SendMessageAsync(string user, string message) =>
        await Clients.All.SendAsync("ReceiveMessage", user, message);

    //вызывается
    public async Task NotifyAsync()
    {
        NotifyHelloMessage?.Invoke();
        await Task.CompletedTask;
    }
}
