using ChatMessanger.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatMessanger.Server.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) :base(options)
        {
            
        }
    }
}
