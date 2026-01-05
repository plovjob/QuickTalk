using System.Data.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuickTalk.Identity.Persistence.DbContext;
using Respawn;
using Respawn.Graph;

namespace QuickTalk.Identity.ComponentTests.Services;

public class RespawnerHostedService(IServiceProvider serviceProvider) : IHostedService
{
    public Respawner Respawner = null!;
    private DbConnection connection = null!;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        connection = GetConnection();
        await connection.OpenAsync();

        Respawner = await Respawner.CreateAsync(
            connection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = ["public"],
                TablesToIgnore = [new Table("__migrations")]
            });
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await connection.CloseAsync();
    }

    private DbConnection GetConnection()
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        connection = (DbConnection)dbContext.CreateConnection();
        return connection;
    }
}
