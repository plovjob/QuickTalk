using System.Data;
using System.Globalization;
using System.IO.Abstractions;
using System.Text.RegularExpressions;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using QuickTalk.Identity.Persistence.DbContext;

namespace QuickTalk.Identity.Persistence.Migrations;

public class MigrationRunner(
    IServiceProvider serviceProvider,
    IFileSystem fileSystem,
    ILogger<MigrationRunner> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        await dbContext.InitAsync();
        await MigrateAsync(dbContext);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task MigrateAsync(UserDbContext dbContext)
    {
        using var connection = dbContext.CreateConnection();

        await connection!.OpenAsync();
        await EnsureMigrationTableExistsAsync(connection);

        var appliedMigrations = await GetAppliedMigrationsAsync(connection);

        var path = Path.Combine(AppContext.BaseDirectory, "Migrations");
        var migrationFiles = fileSystem.Directory.GetFiles(path, "*.sql")
            .OrderBy(f => f)
            .ToArray();

        foreach (var filePath in migrationFiles)
        {
            var migrationInfo = ExtractMigrationInfo(filePath);

            if (!appliedMigrations.Contains(migrationInfo.Version))
            {
                await ApplyMigrationAsync(connection, filePath, migrationInfo);
            }
        }
    }

    private (int Version, string Name) ExtractMigrationInfo(string filePath)
    {
        var fileName = Path.GetFileNameWithoutExtension(filePath);
        var match = Regex.Match(fileName, @"^(\d+)_(.+)$");

        if (!match.Success)
        {
            throw new FormatException($"Invalid migration file name: {fileName}");
        }

        var version = int.Parse(match.Groups[1].Value, CultureInfo.CurrentCulture);
        var name = match.Groups[2].Value;

        return (version, name);
    }

    private async Task ApplyMigrationAsync(NpgsqlConnection connection, string filePath,
        (int Version, string Name) migrationInfo)
    {
        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var sql = await fileSystem.File.ReadAllTextAsync(filePath);

            await connection.ExecuteAsync(sql, transaction: transaction);

            await connection.ExecuteAsync(
                "INSERT INTO __migrations (version, name) VALUES (@Version, @Name)",
                new { migrationInfo.Version, migrationInfo.Name },
                transaction: transaction);

            await transaction.CommitAsync();

            logger.LogInformation($"Applied migration: {migrationInfo.Version}_{migrationInfo.Name}");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception($"Failed to apply migration {migrationInfo.Version}: {ex.Message}", ex);
        }
    }

    private async Task<HashSet<int>> GetAppliedMigrationsAsync(IDbConnection connection)
    {
        var sql = "SELECT * FROM __migrations ORDER BY version;";
        var result = await connection.QueryAsync<int>(sql);
        return [.. result];
    }

    private async Task EnsureMigrationTableExistsAsync(IDbConnection connection)
    {
        var sql = @"CREATE TABLE IF NOT EXISTS __migrations (
                migration_id SERIAL PRIMARY KEY,
                version INTEGER NOT NULL UNIQUE,
                name VARCHAR(255) NOT NULL,
                applied_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP)";

        await connection.ExecuteAsync(sql);
    }
}
