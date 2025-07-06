using Microsoft.EntityFrameworkCore;
using QuickTalk.Messages.Domain.Entities;

namespace QuickTalk.Messages.Persistence
{
    public class MessageDbContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }
        public MessageDbContext(DbContextOptions options) : base(options)
        {
            
        }
    }
}
