using MediatR;
using QuickTalk.Messages.Domain.Entities;
using QuickTalk.Messages.Domain.Interfaces;

namespace QuickTalk.Messages.Application.Queries.GetAllMessagesAsync;

public sealed class GetAllMessagesAsyncRequestHandler(IMessageRepository messageRepository) :
    IRequestHandler<GetAllMessagesAsyncRequest, OperationResult<IReadOnlyCollection<Message>>>
{
    public async Task<OperationResult<IReadOnlyCollection<Message>>> Handle(
        GetAllMessagesAsyncRequest request,
        CancellationToken cancellationToken = default)
    {
        return await messageRepository.ListAsync();
    }
}
