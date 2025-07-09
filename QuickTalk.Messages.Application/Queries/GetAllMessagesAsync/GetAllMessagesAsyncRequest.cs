using MediatR;
using QuickTalk.Messages.Domain.Dto;

namespace QuickTalk.Messages.Application.Queries.GetAllMessagesAsync;

public class GetAllMessagesAsyncRequest : IRequest<IEnumerable<MessageDto>>;
