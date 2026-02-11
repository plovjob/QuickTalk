using MassTransit;
using QuickTalk.Messages.Domain.Entities;
using QuickTalk.Messages.Domain.Interfaces;
using QuickTalk.Shared.Messaging;

namespace QuickTalk.Messages.WebApi.Consumers;

public class NewUserRegisteredConsumer(IMessageRepository userRepository) : IConsumer<IUserRegistered>
{
    public async Task Consume(ConsumeContext<IUserRegistered> context)
    {
        var id = context.Message.Id;
        var userName = context.Message.UserName;
        var user = new MessengerUser(id, userName);

        await userRepository.CreateNewUserAsync(user);
    }
}
