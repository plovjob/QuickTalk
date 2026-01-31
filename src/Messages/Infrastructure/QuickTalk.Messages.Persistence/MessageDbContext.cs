using Microsoft.EntityFrameworkCore;
using QuickTalk.Messages.Domain.Entities;
using QuickTalk.Messages.Persistence.EntityConfiguration;

namespace QuickTalk.Messages.Persistence;

public sealed class MessageDbContext(DbContextOptions<MessageDbContext> options) : DbContext(options)
{
    public DbSet<Message> Messages { get; set; } = null!;
    public DbSet<MessangerUser> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MessageEntityConfiguration());

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasOne(m => m.FromUser)
            .WithMany(u => u.MessagesFromUsers)
            .HasForeignKey(m => m.FromUserId);

            entity.HasOne(m => m.ToUser)
            .WithMany(u => u.MessagesToUsers)
            .HasForeignKey(m => m.ToUserId);
        });
    }
}
