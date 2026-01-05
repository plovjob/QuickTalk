using Microsoft.EntityFrameworkCore;
using QuickTalk.Messages.Domain.Entities;
using QuickTalk.Messages.Domain.Interfaces;

namespace QuickTalk.Messages.Persistence.Repository;

public sealed class MessageRepository(MessageDbContext messageDbContext) : IMessageRepository
{
    public async Task<Message?> GetMessageByIdAsync(Guid id)
    {
        return await messageDbContext.Messages.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<OperationResult> CreateAsync(Message message)
    {
        var addedMessage = await messageDbContext.Messages.AddAsync(message);

        if (addedMessage == null)
        {
            return OperationResult.Failure(InternalError.ValueNotAdded("Message not added to database"));
        }

        await messageDbContext.SaveChangesAsync();
        return OperationResult.Success();
    }

    public async Task<OperationResult> UpdateAsync(Message message)
    {
        var messageEntity = await messageDbContext.Messages.FindAsync(message.Id);

        if (messageEntity == null)
        {
            return OperationResult.Failure(InternalError.NotFound("Message not found"));
        }

        if (message.EditedAt.HasValue)
        {
            messageEntity.UpdateEditedAt(message.EditedAt.Value);
        }

        messageEntity.UpdateText(message.Text);

        await messageDbContext.SaveChangesAsync();
        return OperationResult.Success(message);
    }

    public async Task<OperationResult<IReadOnlyCollection<Message>>> ListAsync()
    {
        var messages = await messageDbContext.Messages.AsNoTracking().ToListAsync();

        if (messages == null)
        {
            return OperationResult.Failure<IReadOnlyCollection<Message>>(InternalError.CollectionDoesNotExists("Collection does not exists"));
        }

        var readOnlyMessage = messages.AsReadOnly();

        return OperationResult.Success<IReadOnlyCollection<Message>>(readOnlyMessage);
    }
}
