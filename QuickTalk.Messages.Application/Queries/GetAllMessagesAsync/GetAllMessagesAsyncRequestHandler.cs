using MediatR;
using QuickTalk.Messages.Domain.Entities;
using QuickTalk.Messages.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickTalk.Messages.Application.Queries.GetAllMessagesAsync
{
    public class GetAllMessagesAsyncRequestHandler : IRequestHandler<GetAllMessagesAsyncRequest, IList<Message>>
    {
        private readonly IMessageRepository _messageRepository;
        public GetAllMessagesAsyncRequestHandler(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }
        public async Task<IList<Message>> Handle(GetAllMessagesAsyncRequest request, CancellationToken cancellationToken = default)
        {
            return await _messageRepository.GetAllMessagesAsync();
        }
    }
}
