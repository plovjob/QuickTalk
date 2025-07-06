using MediatR;
using QuickTalk.Messages.Domain.Entities;


namespace QuickTalk.Messages.Application.Queries.GetAllMessagesAsync
{
    public class GetAllMessagesAsyncRequest : IRequest<IList<Message>> { }
}
