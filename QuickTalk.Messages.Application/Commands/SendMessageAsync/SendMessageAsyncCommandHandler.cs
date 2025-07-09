using MediatR;
using QuickTalk.Messages.Domain.Entities;
using QuickTalk.Messages.Domain.Interfaces;

namespace QuickTalk.Messages.Application.Commands.SendMessage;

public class SendMessageAsyncCommandHandler(IMessageRepository messageRepository) : IRequestHandler<SendMessageAsyncCommand>
{
    public async Task Handle(SendMessageAsyncCommand request, CancellationToken cancellationToken = default)
    {
        var messageDto = request.Message;
        var message = new Message(
            messageDto.UserName,
            messageDto.Text,
            TimeProvider.System.GetUtcNow().DateTime);
        await messageRepository.SendMessageAsync(message);
    }
}
