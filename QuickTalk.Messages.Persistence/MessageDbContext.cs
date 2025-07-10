using Microsoft.EntityFrameworkCore;
using QuickTalk.Messages.Domain.Entities;

namespace QuickTalk.Messages.Persistence;

public class MessageDbContext(DbContextOptions<MessageDbContext> options) : DbContext(options)
{
    public DbSet<Message> Messages { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Message>(p =>
        {
            p.HasKey(e => e.Id);

            p.Property(e => e.UserName);
            p.Property(e => e.Text);
            p.Property(e => e.SentAt);
        });
    }
}
