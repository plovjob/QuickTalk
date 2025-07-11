using MediatR;
using QuickTalk.Messages.Domain.Dto;
using QuickTalk.Messages.Domain.Interfaces;

namespace QuickTalk.Messages.Application.Queries.GetAllMessagesAsync;

public sealed class GetAllMessagesAsyncRequestHandler(IMessageRepository messageRepository) : IRequestHandler<GetAllMessagesAsyncRequest, IEnumerable<MessageDto>>
{
    public async Task<IEnumerable<MessageDto>> Handle(GetAllMessagesAsyncRequest request, CancellationToken cancellationToken = default)
    {
        var messages = await messageRepository.GetAllMessagesAsync();
        return messages.Select(e => new MessageDto(e.UserName, e.Text, e.SentAt));
    }
}
