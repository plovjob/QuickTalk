using MediatR;
using QuickTalk.Messages.Domain.Interfaces;

namespace QuickTalk.Messages.Application.Commands.SendMessage
{
    public class SendMessageAsyncCommandHandler : IRequestHandler<SendMessageAsyncCommand>
    {
        private readonly IMessageRepository _messageRepository;
        public SendMessageAsyncCommandHandler(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task Handle(SendMessageAsyncCommand request, CancellationToken cancellationToken = default)
        {
            await _messageRepository.SendMessageAsync(request.Message);
        }
    }
}
