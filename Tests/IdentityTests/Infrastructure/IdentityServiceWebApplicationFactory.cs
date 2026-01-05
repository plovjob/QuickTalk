using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using QuickTalk.Identity.ComponentTests.Extensions;
using QuickTalk.Identity.ComponentTests.Services;
using QuickTalk.Identity.Persistence;
using Testcontainers.PostgreSql;

namespace QuickTalk.Identity.ComponentTests.Infrastructure;

public class IdentityServiceWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private DataBaseConfiguration _testConfigurationInstance = null!;
    private PostgreSqlContainer _postgres = null!;
    private string _connectionString = null!;

    public async Task InitializeAsync()
    {
        _postgres = new PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpass")
            .WithCleanUp(true)
            .WithPortBinding(6543, 5432)
            .Build();

        await _postgres.StartAsync();

        _connectionString = _postgres.GetConnectionString();
    }

    public new async Task DisposeAsync()
    {
        await _postgres.StopAsync();
        await _postgres.DisposeAsync();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(config =>
        {
            var dbConfiguration = ParseContainerConnectionString(_connectionString);
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:TestConnection:Host"] = dbConfiguration.Host,
                ["ConnectionStrings:TestConnection:Port"] = dbConfiguration.Port,
                ["ConnectionStrings:TestConnection:DataBase"] = dbConfiguration.Database,
                ["ConnectionStrings:TestConnection:UserName"] = dbConfiguration.UserName,
                ["ConnectionStrings:TestConnection:Password"] = dbConfiguration.Password,
                ["Environment"] = "Testing",
                ["Logging:LogLevel:Default"] = "Warning"
            });

            var configuration = config.Build();
            _testConfigurationInstance = new DataBaseConfiguration();
            var section = configuration.GetSection("ConnectionStrings:TestConnection");

            configuration.GetSection("ConnectionStrings:TestConnection").Bind(_testConfigurationInstance);
        });

        builder.ConfigureTestServices(services =>
        {
            services.AddFakeTimeProvider();

            services.AddHostedService<RespawnerHostedService>();

            var configurationDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(DataBaseConfiguration));

            if (configurationDescriptor != null)
            {
                services.Remove(configurationDescriptor);
            }

            services.AddSingleton(_testConfigurationInstance);

            services.AddHangfireTestingService();

            services.AddHangfireRefreshTokenMockJob();
        });
    }

    private DataBaseConfiguration ParseContainerConnectionString(string postgresContainerConnectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(postgresContainerConnectionString);
        return new DataBaseConfiguration()
        {
            Host = builder.Host,
            Port = builder.Port.ToString(CultureInfo.InvariantCulture),
            Database = builder.Database,
            UserName = builder.Username,
            Password = builder.Password
        };
    }
}
