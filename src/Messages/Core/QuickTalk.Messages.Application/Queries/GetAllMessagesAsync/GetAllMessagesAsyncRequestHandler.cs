using MediatR;
using QuickTalk.Messages.Domain.Dto;
using QuickTalk.Messages.Domain.Entities;
using QuickTalk.Messages.Domain.Interfaces;

namespace QuickTalk.Messages.Application.Queries.GetAllMessagesAsync;

public sealed class GetAllMessagesAsyncRequestHandler(IMessageRepository messageRepository) :
    IRequestHandler<GetAllMessagesAsyncRequest, OperationResult<IReadOnlyCollection<MessageDto>>>
{
    public async Task<OperationResult<IReadOnlyCollection<MessageDto>>> Handle(
        GetAllMessagesAsyncRequest request,
        CancellationToken cancellationToken = default)
    {
        return await messageRepository.GetAllMessagesAsync();
    }
}
