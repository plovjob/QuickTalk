using Dapper;
using Npgsql;
using QuickTalk.Identity.ComponentTests.Mock.Interfaces;
using QuickTalk.Identity.Persistence;

namespace QuickTalk.Identity.ComponentTests.Mock.Services;

public class MockRefreshTokenCleanUpService(DataBaseConfiguration connectionString, TimeProvider timeProvider) : IMockRefreshTokenCleanUpService
{
    public async Task ClearExpiredTokensAsync()
    {
        using var connection = new NpgsqlConnection(connectionString.ToString());

        if (connection is null)
        {
            throw new InvalidCastException($"Невозможно преобразовать {connection?.GetType().Name} к NpgsqlConnection");
        }

        await connection.OpenAsync();

        var sql = "DELETE FROM RefreshToken WHERE RefreshToken.ExpiresAt < @timeNow;";

        var commandResult = await connection.ExecuteAsync(sql, new { timeNow = timeProvider.GetUtcNow() });
    }
}
