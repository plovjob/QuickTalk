using Microsoft.EntityFrameworkCore;
using QuickTalk.Messages.Domain.Entities;
using QuickTalk.Messages.Domain.Interfaces;

namespace QuickTalk.Messages.Persistence.Repository;

public sealed class MessageRepository(MessageDbContext messageDbContext) : IMessageRepository
{
    public async Task<IEnumerable<Message>> GetAllMessagesAsync()
    {
        return await messageDbContext.Messages.ToListAsync();
    }

    public async Task SendMessageAsync(Message message)
    {
        await messageDbContext.Messages.AddAsync(message);
        await messageDbContext.SaveChangesAsync();
    }
}
