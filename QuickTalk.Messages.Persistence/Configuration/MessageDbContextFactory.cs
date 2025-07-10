using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace QuickTalk.Messages.Persistence.Configuration;

internal class MessageDbContextFactory : IDesignTimeDbContextFactory<MessageDbContext>
{
    public MessageDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()), "..QuickTalk.Messages.WebApi")
    }
}
