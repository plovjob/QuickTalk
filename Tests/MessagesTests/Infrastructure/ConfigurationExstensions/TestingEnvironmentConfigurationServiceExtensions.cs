using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using QuickTalk.Messages.ComponentTests.Infrastructure.MockTime;
using QuickTalk.Messages.Persistence;

namespace QuickTalk.Messages.ComponentTests.Infrastructure.ConfigurationExstensions;

public static class TestingEnvironmentConfigurationServiceExtensions
{
    public static IWebHostBuilder AddInMemoryDbContext(this IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(d =>
            d.ServiceType == typeof(IDbContextOptionsConfiguration<MessageDbContext>));
            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            services.AddDbContext<MessageDbContext>(options =>
            {
                options.UseInMemoryDatabase("MessagesTestDatabase");
            });
        });

        return builder;
    }

    public static IWebHostBuilder AddTestTimeService(this IWebHostBuilder builder, TimeConfigurationObject timeConfiguration)
    {
        builder.ConfigureServices(services =>
        {
            var timeDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(TimeProvider));
            if (timeDescriptor != null)
            {
                services.Remove(timeDescriptor);
            }

            services.AddSingleton<TimeProvider>(sp => new FakeTimeProvider(timeConfiguration));
        });

        return builder;
    }
}
