using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuickTalk.Messages.Domain.Interfaces;
using QuickTalk.Messages.Persistence.Repository;

namespace QuickTalk.Messages.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistenceDependencies(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<MessageDbContext>(options =>
        {
            options.UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
