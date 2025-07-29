using MediatR;
using QuickTalk.Messages.Domain.Dto;
using QuickTalk.Messages.Domain.Entities;

namespace QuickTalk.Messages.Application.Commands.SendMessage;

public sealed class SendMessageAsyncCommand(MessageDto messageDto) :
    IRequest<OperationResult<MessageDto>>
{
    public MessageDto Message { get; } = messageDto;
}
