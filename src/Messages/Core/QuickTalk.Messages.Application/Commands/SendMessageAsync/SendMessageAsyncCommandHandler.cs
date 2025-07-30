using MediatR;
using QuickTalk.Messages.Domain.Dto;
using QuickTalk.Messages.Domain.Entities;
using QuickTalk.Messages.Domain.Interfaces;

namespace QuickTalk.Messages.Application.Commands.SendMessage;

public class SendMessageAsyncCommandHandler(IMessageRepository messageRepository) : IRequestHandler<SendMessageAsyncCommand, OperationResult<MessageDto>>
{
    public async Task<OperationResult<MessageDto>> Handle(SendMessageAsyncCommand request, CancellationToken cancellationToken = default)
    {
        var messageDto = request.Message;
        return await messageRepository.SendMessageAsync(messageDto);
    }
}
