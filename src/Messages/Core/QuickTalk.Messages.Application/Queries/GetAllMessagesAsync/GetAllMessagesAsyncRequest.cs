using MediatR;
using QuickTalk.Messages.Domain.Entities;

namespace QuickTalk.Messages.Application.Queries.GetAllMessagesAsync;

public sealed class GetAllMessagesAsyncRequest(Guid senderId, Guid consumerId) : IRequest<OperationResult<IReadOnlyCollection<Message>>>
{
    public Guid SenderId { get; private set; } = senderId;
    public Guid ConsumerId { get; private set; } = consumerId;
}
