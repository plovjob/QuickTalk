using Dapper;
using Npgsql;

namespace QuickTalk.Identity.Persistence.DbContext;

public class UserDbContext(DataBaseConfiguration dbConfiguration)
{
    public NpgsqlConnection CreateConnection()
    {
        var connectionString = dbConfiguration.ToString();
        return new NpgsqlConnection(connectionString);
    }

    public async Task InitAsync()
    {
        await InitDatabaseAsync();
    }

    private async Task InitDatabaseAsync()
    {
        var connectionString = (dbConfiguration with { Database = "postgres" }).ToString();
        using var connection = new NpgsqlConnection(connectionString);
        var sqlDbCountQuery = $"SELECT COUNT(*) FROM pg_database WHERE datname = '{dbConfiguration.Database}';";
        var dbCount = await connection.ExecuteScalarAsync<int>(sqlDbCountQuery);
        if (dbCount == 0)
        {
            var sql = $"CREATE DATABASE \"{dbConfiguration.Database}\";";
            await connection.ExecuteAsync(sql);
        }
    }
}
