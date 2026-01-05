using Dapper;
using Npgsql;
using QuickTalk.Identity.Domain.Entities;
using QuickTalk.Identity.Persistence.DbContext;
using QuickTalk.Identity.Persistence.Interfaces;

namespace QuickTalk.Identity.Persistence.Repository;

public class RefreshTokenRepository(UserDbContext context, TimeProvider timeProvider) : IRefreshTokenRepository
{
    public async Task ClearExpiredTokensAsync()
    {
        using var connection = context.CreateConnection();

        await connection.OpenAsync();

        var sql = "DELETE FROM RefreshToken WHERE RefreshToken.ExpiresAt < @timeNow;";

        var commandResult = await connection.ExecuteAsync(sql, new { timeNow = timeProvider.GetUtcNow() });
    }

    public async Task<Result> AddUserRefreshTokenAsync(RefreshToken token)
    {
        using var connection = context.CreateConnection();

        await connection.OpenAsync();

        var searchUserExistingRefreshTokenResult = await InnerGetUserRefreshTokenAsync(connection, token.UserId);

        if (searchUserExistingRefreshTokenResult.IsSuccess &&
            searchUserExistingRefreshTokenResult.Data != null)
        {
            return Result.Failure(UserErrors.RefreshTokenAlreadyExists(token.UserId));
        }

        return await InnerAddRefreshTokenAsync(connection, token);
    }

    public async Task<Result> UpdateRefreshTokenAsync(RefreshToken token)
    {
        using var connection = context.CreateConnection();

        await connection.OpenAsync();

        var searchUserExistingRefreshTokenResult = await InnerGetUserRefreshTokenAsync(connection, token.UserId);

        if (searchUserExistingRefreshTokenResult.IsFailure)
        {
            return Result.Failure(searchUserExistingRefreshTokenResult.Error!);
        }

        if (searchUserExistingRefreshTokenResult.Data == null)
        {
            return Result.Failure(UserErrors.RefreshTokenNotFound(token.UserId));
        }

        try
        {
            var deleteTokenCommand = "DELETE FROM RefreshToken WHERE RefreshToken.UserId = @Id";
            var commandResult = await connection.ExecuteAsync(deleteTokenCommand, new { Id = token.UserId });
            return await InnerAddRefreshTokenAsync(connection, token);
        }
        catch (Exception ex)
        {
            return Result<RefreshToken?>.Failure(UserErrors.InternalError(ex.Message));
        }
    }

    public async Task<Result<RefreshToken?>> GetByUserIdAsync(Guid id)
    {
        try
        {
            using var connection = context.CreateConnection();

            await connection.OpenAsync();

            var getUserRefreshTokenInfoCommand = "SELECT " +
                "t.UserId, t.Token, t.IssuedAt, t.ExpiresAt " +
                "FROM RefreshToken t " +
                "WHERE t.UserId = @id;";

            var refreshToken = await connection.QuerySingleOrDefaultAsync<RefreshToken>(getUserRefreshTokenInfoCommand, new { id });

            return Result<RefreshToken?>.Success(refreshToken);
        }
        catch (Exception ex)
        {
            return Result<RefreshToken?>.Failure(UserErrors.InternalError(ex.Message));
        }
    }

    public async Task<Result<RefreshToken?>> GetByTokenAsync(string refreshToken)
    {
        try
        {
            using var connection = context.CreateConnection();

            await connection.OpenAsync();

            var getRefreshTokenCommand = "SELECT " +
                "t.UserId, t.Token, t.IssuedAt, t.ExpiresAt " +
                "FROM RefreshToken t " +
                "WHERE t.Token = @token;";

            var refreshTokenObject = await connection.QuerySingleOrDefaultAsync<RefreshToken>(getRefreshTokenCommand, new { refreshToken });

            return Result<RefreshToken?>.Success(refreshTokenObject);
        }
        catch (Exception ex)
        {
            return Result<RefreshToken?>.Failure(UserErrors.InternalError(ex.Message));
        }
    }

    private async Task<Result<RefreshToken?>> InnerGetUserRefreshTokenAsync(NpgsqlConnection connection, Guid id)
    {
        try
        {
            var getUserRefreshTokenInfoCommand = "SELECT " +
          "t.UserId, t.Token, t.IssuedAt, t.ExpiresAt " +
          "FROM RefreshToken t " +
          "WHERE t.UserId = @id;";

            var refreshToken = await connection.QuerySingleOrDefaultAsync<RefreshToken>(getUserRefreshTokenInfoCommand, new { id });

            return Result<RefreshToken?>.Success(refreshToken);
        }
        catch (Exception ex)
        {
            return Result<RefreshToken?>.Failure(UserErrors.InternalError(ex.Message));
        }
    }

    private async Task<Result> InnerAddRefreshTokenAsync(NpgsqlConnection connection, RefreshToken token)
    {
        try
        {
            var addUserRefreshTokenInfoCommand = "INSERT INTO RefreshToken (UserId, Token, IssuedAt, ExpiresAt) " +
           "VALUES (@UserId, @Token, @IssuedAt, @ExpiresAt);";

            var addUserRefreshTokenInfoParameters = new
            {
                UserId = token.UserId,
                Token = token.Token,
                IssuedAt = token.IssuedAt,
                ExpiresAt = token.ExpiresAt
            };

            await connection.ExecuteAsync(addUserRefreshTokenInfoCommand, addUserRefreshTokenInfoParameters);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result<RefreshToken?>.Failure(UserErrors.InternalError(ex.Message));
        }
    }
}
