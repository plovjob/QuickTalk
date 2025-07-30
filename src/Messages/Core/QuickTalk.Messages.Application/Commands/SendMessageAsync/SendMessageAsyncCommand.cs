using MediatR;
using QuickTalk.Messages.Domain.Dto;
using QuickTalk.Messages.Domain.Entities;

namespace QuickTalk.Messages.Application.Commands.SendMessage;

public class SendMessageAsyncCommand(MessageDto message) :
    IRequest<OperationResult<MessageDto>>
{
    public MessageDto Message { get; } = message;
}
