using MediatR;
using QuickTalk.Messages.Domain.Dto;

namespace QuickTalk.Messages.Application.Commands.SendMessage;

public sealed class SendMessageAsyncCommand(MessageDto Message) : IRequest
{
    public MessageDto Message { get; } = Message;
}
