using QuickTalk.Messages.Domain.Entities;

namespace QuickTalk.Messages.Domain.Interfaces;

public interface IMessageRepository
{
    Task<OperationResult> CreateAsync(Message message);
    Task<OperationResult<IReadOnlyCollection<Message>>> ListAsync();
    Task<OperationResult> UpdateAsync(Message message);
    Task<Message?> GetMessageByIdAsync(Guid id);
}
