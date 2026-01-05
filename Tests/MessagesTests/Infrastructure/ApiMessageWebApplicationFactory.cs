using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using QuickTalk.Messages.ComponentTests.Infrastructure.ConfigurationExstensions;
using QuickTalk.Messages.ComponentTests.Infrastructure.MockTime;
using QuickTalk.Messages.Persistence;

namespace QuickTalk.Messages.ComponentTests.Infrastructure;

public class ApiMessageWebApplicationFactory : WebApplicationFactory<Program>
{
    public HttpClient HttpClient { get; private set; } = null!;
    public MessageDbContext dbContext { get; set; } = null!;

    public TimeConfigurationObject defaultTestTime { get; private set; } =
        new(new DateTime(2025, 8, 28, 13, 0, 0, kind: DateTimeKind.Utc).ToUniversalTime(), new(0, 1, 0));

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.AddInMemoryDbContext();
        builder.AddTestTimeService(defaultTestTime);
    }

    public async Task InitializeAsync()
    {
        var scope = Services.CreateScope();
        dbContext = scope.ServiceProvider.GetRequiredService<MessageDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        var currentTime = (FakeTimeProvider)scope.ServiceProvider.GetRequiredService<TimeProvider>();
        currentTime.SetToDefault();
    }

    public async Task ResetDatabaseAsync()
    {
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.DisposeAsync();
    }
}
