using MassTransit;
using Microsoft.AspNetCore.SignalR;
using QuickTalk.Messages.WebApi.Hubs;
using QuickTalk.Shared.Messaging;

namespace QuickTalk.Messages.WebApi.Consumers;

public class HelloMessageConsumer(IHubContext<MessageHub> hubContext, MessageHub hub) : IConsumer<IUserHelloMessage>
{
    public string? Message { get; set; }

    public async Task Consume(ConsumeContext<IUserHelloMessage> context)
    {
        hub.CanSendHelloMessage += OnSendHelloMessageAsync;
        Message = context.Message.Message;
        await Task.CompletedTask;
    }

    private async Task OnSendHelloMessageAsync()
    {
        await hubContext.Clients.All.SendAsync("ReceiveHelloMessage", Message);
    }
}
