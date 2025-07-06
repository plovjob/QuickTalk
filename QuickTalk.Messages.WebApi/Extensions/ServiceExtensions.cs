using MediatR;
using Microsoft.EntityFrameworkCore;
using QuickTalk.Messages.Domain.Interfaces;
using QuickTalk.Messages.Persistence;
using QuickTalk.Messages.Persistence.Repository;

namespace QuickTalk.Messages.WebApi.Extensions
{
    //внедряет зависимости основываясь на слое, из которого эти зависимости торчат
    public static class ServiceExtensions
    {
        public static void AddApplicationDependencies(this IServiceCollection services)
        {
            services.AddMediatR(conf => conf.RegisterServicesFromAssembly(typeof(Program).Assembly));
        }

        public static void AddPersistenceDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MessageDbContext>(options
                => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("Messages.Infrastructure.Persistence")));

            services.AddScoped<IMessageRepository, MessageRepository>();
        }
    }
}
