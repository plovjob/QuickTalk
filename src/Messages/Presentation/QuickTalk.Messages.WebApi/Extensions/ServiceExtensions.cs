using Microsoft.EntityFrameworkCore;
using QuickTalk.Messages.Domain.Interfaces;
using QuickTalk.Messages.Persistence;
using QuickTalk.Messages.Persistence.Repository;
using QuickTalk.Messages.Application.Behaviors;
using MediatR;

namespace QuickTalk.Messages.WebApi.Extensions;

internal static class ServiceExtensions
{
    internal static void AddApplicationDependencies(this IServiceCollection services)
    {
        services.AddMediatR(conf =>
        {
            conf.RegisterServicesFromAssembly(typeof(Application.AssemblyReference).Assembly);
            //conf.AddOpenBehavior(typeof(RequestResponseLoggingBehavior<,>));
        });
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestResponseLoggingBehavior<,>));
    }

    internal static void AddPersistenceDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MessageDbContext>(options
            => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly("QuickTalk.Messages.Persistence")));

        services.AddScoped<IMessageRepository, MessageRepository>();
    }
}
