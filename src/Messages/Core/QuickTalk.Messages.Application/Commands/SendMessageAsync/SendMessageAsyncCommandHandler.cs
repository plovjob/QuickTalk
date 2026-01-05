using MediatR;
using QuickTalk.Messages.Domain.Entities;
using QuickTalk.Messages.Domain.Interfaces;

namespace QuickTalk.Messages.Application.Commands.SendMessage;

public class SendMessageAsyncCommandHandler(IMessageRepository messageRepository, TimeProvider timeProvider) : IRequestHandler<SendMessageAsyncCommand, OperationResult>
{
    public async Task<OperationResult> Handle(SendMessageAsyncCommand request, CancellationToken cancellationToken = default)
    {
        var message = request.Message;
        var existingMessage = await messageRepository.GetMessageByIdAsync(message.Id);

        if (existingMessage != null)
        {
            message.UpdateEditedAt(timeProvider.GetUtcNow().UtcDateTime);
            return await messageRepository.UpdateAsync(message);
        }

        var time = timeProvider.GetUtcNow().UtcDateTime;
        message.SetSentAt(time);
        return await messageRepository.CreateAsync(message);
    }
}
