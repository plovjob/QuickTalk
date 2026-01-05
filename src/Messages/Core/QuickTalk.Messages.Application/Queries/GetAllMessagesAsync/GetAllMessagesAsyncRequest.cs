using MediatR;
using QuickTalk.Messages.Domain.Entities;

namespace QuickTalk.Messages.Application.Queries.GetAllMessagesAsync;

public sealed class GetAllMessagesAsyncRequest : IRequest<OperationResult<IReadOnlyCollection<Message>>>;
