using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace QuickTalk.Messages.WebApi.Hubs;

[Authorize]
internal sealed class MessageHub : Hub
{
    //метод SendMessageAsync получает сообщение и имя отправившего его пользователя и ретранслирует его всем подключенным клиентам.
    public async Task SendMessageAsync(string user, string message) =>
        await Clients.All.SendAsync("ReceiveMessage", user, message);
}
