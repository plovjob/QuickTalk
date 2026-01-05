using MassTransit;
//using QuickTalk.Messages.Domain.Interfaces;
using QuickTalk.Shared.Messaging;

namespace QuickTalk.Messages.WebApi.Consumers;

public class PublisherService(/*IMessageRepository messageRepository*/) : IConsumer<IUserLoggedIn>
{
    public async Task Consume(ConsumeContext<IUserLoggedIn> context)
    {
        var message = context.Message;
        await Task.CompletedTask;
    }
}
