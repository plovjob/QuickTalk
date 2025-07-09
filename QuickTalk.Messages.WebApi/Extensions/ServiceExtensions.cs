using Microsoft.EntityFrameworkCore;
using QuickTalk.Messages.Domain.Interfaces;
using QuickTalk.Messages.Persistence;
using QuickTalk.Messages.Persistence.Repository;

namespace QuickTalk.Messages.WebApi.Extensions;

public static class ServiceExtensions
{
    public static void AddApplicationDependencies(this IServiceCollection services)
    {
        services.AddMediatR(conf => conf.RegisterServicesFromAssembly(
                                    typeof(Application.AssemblyReference).Assembly));
    }

    public static void AddPersistenceDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MessageDbContext>(options
            => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly("QuickTalk.Messages.Persistence")));

        services.AddScoped<IMessageRepository, MessageRepository>();
    }
}
