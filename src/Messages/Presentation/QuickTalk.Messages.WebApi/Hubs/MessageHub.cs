using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace QuickTalk.Messages.WebApi.Hubs;

[Authorize]
public sealed class MessageHub : Hub
{
    public event Func<Task>? CanSendHelloMessage;

    public async Task SendMessageAsync(string user, string message) =>
        await Clients.All.SendAsync("ReceiveMessage", user, message);

    public async Task SendHelloMessageAsync()
    {
        CanSendHelloMessage?.Invoke();
        await Task.CompletedTask;
    }
}
