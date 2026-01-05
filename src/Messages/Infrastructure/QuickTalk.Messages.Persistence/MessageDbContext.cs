using Microsoft.EntityFrameworkCore;
using QuickTalk.Messages.Domain.Entities;
using QuickTalk.Messages.Persistence.EntityConfiguration;

namespace QuickTalk.Messages.Persistence;

public sealed class MessageDbContext(DbContextOptions<MessageDbContext> options) : DbContext(options)
{
    public DbSet<Message> Messages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MessageEntityConfiguration());
    }
}
