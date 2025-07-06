using MediatR;
using QuickTalk.Messages.Domain.Entities;

namespace QuickTalk.Messages.Application.Commands.SendMessage
{
    public class SendMessageAsyncCommand : IRequest
    {
        public Message Message { get; set; }
    }
}
