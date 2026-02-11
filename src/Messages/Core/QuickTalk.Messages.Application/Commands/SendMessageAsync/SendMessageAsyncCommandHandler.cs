using MediatR;
using QuickTalk.Messages.Domain.Entities;

namespace QuickTalk.Messages.Application.Commands.SendMessage;

public class SendMessageAsyncCommandHandler : IRequestHandler<SendMessageAsyncCommand, OperationResult>
{
    //(IMessageRepository messageRepository, TimeProvider timeProvider)
    public async Task<OperationResult> Handle(SendMessageAsyncCommand request, CancellationToken cancellationToken = default)
    {
        //var message = request.Message;
        //var user = request.User;

        //var existingMessage = await messageRepository.GetMessageByIdAsync(message.Id);

        //if (existingMessage != null)
        //{
        //    message.UpdateEditedAt(timeProvider.GetUtcNow().UtcDateTime);
        //    return await messageRepository.UpdateAsync(message);
        //}

        //var time = timeProvider.GetUtcNow().UtcDateTime;
        //message.SetSentAt(time);
        //return await messageRepository.CreateNewUser(message);
        return await Task.FromResult(OperationResult.Success());
    }
}
