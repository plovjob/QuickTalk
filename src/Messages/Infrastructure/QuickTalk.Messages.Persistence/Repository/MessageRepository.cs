using Microsoft.EntityFrameworkCore;
using QuickTalk.Messages.Domain.Entities;
using QuickTalk.Messages.Domain.Interfaces;

namespace QuickTalk.Messages.Persistence.Repository;

public sealed class MessageRepository(MessageDbContext context) : IMessageRepository
{
    public async Task<OperationResult> CreateNewUserAsync(MessangerUser user)
    {
        var isUserExists = await context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == user.Id && u.UserName == user.UserName);

        if (isUserExists)
        {
            return OperationResult.Failure(InternalError.UserAlreadyExists($"User with name: {user.UserName} already exists"));
        }

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        return OperationResult.Success();
    }

    public async Task<OperationResult<IReadOnlyCollection<Message>>> GetUserMessagesAsync(Guid senderId, Guid consumerId)
    {
        var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Id == consumerId);

        if (existingUser is null)
        {
            return OperationResult.Failure<IReadOnlyCollection<Message>>(InternalError.UserAlreadyExists($"User with Id: {consumerId} does not exists"));
        }

        var messages = existingUser.MessagesFromUsers!
            .Concat(existingUser.MessagesToUsers!)
            .OrderBy(m => m.SentAt)
            .ToList()
            .AsReadOnly();

        return OperationResult.Success<IReadOnlyCollection<Message>>(messages);
    }

    public async Task<OperationResult> SaveUserMessageAsync(Message message)
    {
        var existingUser = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == message.ToUserId);

        if (existingUser is null)
        {
            return OperationResult.Failure(InternalError.UserDoesNotExists($"User with Id: {message.ToUserId} does not exists"));
        }

        await context.Messages.AddAsync(message);
        await context.SaveChangesAsync();

        return OperationResult.Success();
    }

    public async Task<OperationResult> UpdateAsync(Message message)
    {
        var messageEntity = await context.Messages.FindAsync(message.Id);

        if (messageEntity == null)
        {
            return OperationResult.Failure(InternalError.NotFound("Message not found"));
        }

        if (message.EditedAt.HasValue)
        {
            messageEntity.UpdateEditedAt(message.EditedAt.Value);
        }

        messageEntity.UpdateText(message.Text);

        await context.SaveChangesAsync();
        return OperationResult.Success(message);
    }
}
