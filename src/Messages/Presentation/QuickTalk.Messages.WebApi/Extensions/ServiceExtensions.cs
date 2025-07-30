using MediatR;
using Microsoft.EntityFrameworkCore;
using QuickTalk.Messages.Application.Behaviors;
using QuickTalk.Messages.Application.Commands.SendMessage;
using QuickTalk.Messages.Application.Queries.GetAllMessagesAsync;
using QuickTalk.Messages.Domain.Dto;
using QuickTalk.Messages.Domain.Entities;
using QuickTalk.Messages.Domain.Interfaces;
using QuickTalk.Messages.Persistence;
using QuickTalk.Messages.Persistence.Repository;

namespace QuickTalk.Messages.WebApi.Extensions;

internal static class ServiceExtensions
{
    internal static void AddApplicationDependencies(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Application.AssemblyReference).Assembly);
        });
        //инжект работает только если явно объявить используемые типы, похоже что
        //дефолтный контейнер не поддерживает конструкцию:
        //services.AddMediatR(cfg =>
        //{
        //    cfg.RegisterServicesFromAssembly(typeof(Application.AssemblyReference).Assembly);
        //    cfg.AddOpenBehavior(typeof(RequestResponseLoggingBehavior<,>)); <- DI обходит конвейер
        //});
        services.AddTransient<IPipelineBehavior<SendMessageAsyncCommand, OperationResult<MessageDto>>, RequestResponseLoggingBehavior<SendMessageAsyncCommand, MessageDto>>();

        services.AddTransient<IPipelineBehavior<GetAllMessagesAsyncRequest, OperationResult<IReadOnlyCollection<MessageDto>>>, RequestResponseLoggingBehavior<GetAllMessagesAsyncRequest, IReadOnlyCollection<MessageDto>>>();
    }

    internal static void AddPersistenceDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MessageDbContext>(options
            => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly("QuickTalk.Messages.Persistence")));

        services.AddScoped<IMessageRepository, MessageRepository>();
    }
}
