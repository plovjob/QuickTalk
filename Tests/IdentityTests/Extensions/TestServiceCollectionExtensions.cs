using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using QuickTalk.Identity.Application.Services;
using QuickTalk.Identity.ComponentTests.Mock.Interfaces;
using QuickTalk.Identity.ComponentTests.Mock.Services;
using StackExchange.Redis;

namespace QuickTalk.Identity.ComponentTests.Extensions;

public static class TestServiceCollectionExtensions
{
    public static IServiceCollection AddHangfireTestingService(this IServiceCollection services)
    {
        var multiplexerDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IConnectionMultiplexer));

        if (multiplexerDescriptor == null)
        {
            throw new InvalidOperationException();
        }

        services.Remove(multiplexerDescriptor);

#pragma warning disable RCS1124

        var hangfireDescriptors = services.Where(d => d.ServiceType.ToString().StartsWith("Hangfire", StringComparison.OrdinalIgnoreCase)).ToList();

        foreach (var descriptor in hangfireDescriptors)
        {
            services.Remove(descriptor);
        }

#pragma warning restore RCS1124

        services.AddHangfire(options =>
        {
            options.UseInMemoryStorage();
        });

        return services;
    }

    public static IServiceCollection AddFakeTimeProvider(this IServiceCollection services)
    {
        var timeProviderDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(TimeProvider));
        if (timeProviderDescriptor != null)
        {
            services.Remove(timeProviderDescriptor);
        }

        services.AddSingleton<TimeProvider>(sp => new FakeTimeProvider());

        return services;
    }

    public static IServiceCollection AddHangfireRefreshTokenMockJob(this IServiceCollection services)
    {
        var tokenCleanUpServiceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(RefreshTokenService));

        if (tokenCleanUpServiceDescriptor != null)
        {
            services.Remove(tokenCleanUpServiceDescriptor);
        }

        services.AddSingleton<IMockRefreshTokenCleanUpService, MockRefreshTokenCleanUpService>();

        return services;
    }
}
