using MediatR;
using QuickTalk.Messages.Domain.Entities;

namespace QuickTalk.Messages.Application.Commands.SendMessage;

public class SendMessageAsyncCommand(Message message, MessangerUser user) :
    IRequest<OperationResult>
{
    public Message Message { get; } = message;
    public MessangerUser User { get; set; } = user;
}
