using MediatR;
using QuickTalk.Messages.Application.Behaviors;
using QuickTalk.Messages.Application.Commands.SendMessage;
using QuickTalk.Messages.Application.Queries.GetAllMessagesAsync;
using QuickTalk.Messages.Domain.Entities;

namespace QuickTalk.Messages.WebApi.Extensions;

internal static class ServiceExtensions
{
    internal static void AddApplicationDependencies(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Application.AssemblyReference).Assembly);
        });

        services.AddTransient<IPipelineBehavior<SendMessageAsyncCommand, OperationResult>, RequestResponseLoggingBehavior<SendMessageAsyncCommand, OperationResult>>();

        services.AddTransient<IPipelineBehavior<GetAllMessagesAsyncRequest, OperationResult<IReadOnlyCollection<Message>>>, RequestResponseLoggingBehavior<GetAllMessagesAsyncRequest, OperationResult<IReadOnlyCollection<Message>>>>();
    }
}
