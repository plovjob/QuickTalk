using Dapper;
using QuickTalk.Identity.Domain.Entities;
using QuickTalk.Identity.Persistence.DbContext;
using QuickTalk.Identity.Persistence.Interfaces;

namespace QuickTalk.Identity.Persistence.Repository;

public class UserRepository(UserDbContext context) : IUserRepository
{
    public async Task<Result> CreateUserRelatedDataAsync(User user, RefreshToken token)
    {
        using var connection = context.CreateConnection();

        await connection.OpenAsync();
        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var insertUserInfoCommand = "INSERT INTO UsersInfo (Id, CreatedAt) VALUES (@Id, @CreatedAt);";
            var insertUserInfoParameters = new
            {
                Id = user.Id,
                CreatedAt = user.CreatedAt
            };

            var insertUserLoginInfoCommand = "INSERT INTO UsersLoginInfo " +
                "(UserId, Email, UserName, PasswordHash, PasswordSalt) " +
                "VALUES (@UserId, @Email, @UserName, @PasswordHash, @PasswordSalt);";
            var insertUserLoginInfoParameters = new
            {
                UserID = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                PasswordHash = user.PasswordHash,
                PasswordSalt = user.PasswordSalt
            };

            var insertUserRefreshTokenInfoCommand = "INSERT INTO RefreshToken (UserId, Token, IssuedAt, ExpiresAt) " +
            "VALUES (@UserId, @Token, @IssuedAt, @ExpiresAt);";
            var insertUserRefreshTokenInfoParameters = new
            {
                UserId = token.UserId,
                Token = token.Token,
                IssuedAt = token.IssuedAt,
                ExpiresAt = token.ExpiresAt
            };

            await connection.ExecuteAsync(
                insertUserInfoCommand,
                insertUserInfoParameters,
                transaction: transaction);

            await connection.ExecuteAsync(
                insertUserLoginInfoCommand,
                insertUserLoginInfoParameters,
                transaction: transaction);

            await connection.ExecuteAsync(
                insertUserRefreshTokenInfoCommand,
                insertUserRefreshTokenInfoParameters,
                transaction: transaction);

            await transaction.CommitAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            return Result.Failure(UserErrors.InternalError(ex.Message));
        }
    }

    public async Task<Result<User?>> GetUserByEmailAsync(string email)
    {
        try
        {
            using var connection = context.CreateConnection();

            await connection.OpenAsync();

            var getUserByEmailCommand = "SELECT " +
                "u.Id, u.CreatedAt," +
                "ul.UserNAme, ul.Email, ul.PasswordHash, ul.PasswordSalt " +
                "FROM UsersInfo AS u " +
                "INNER JOIN UsersLoginInfo AS ul ON u.Id = ul.UserId " +
                "WHERE ul.Email = @email;";

            var user = await connection.QuerySingleOrDefaultAsync<User>(getUserByEmailCommand, new { email });

            return Result<User?>.Success(user);
        }
        catch (Exception ex)
        {
            return Result<User?>.Failure(UserErrors.InternalError(ex.Message));
        }
    }

    public async Task<Result<User?>> GetUserByIdAsync(Guid id)
    {
        try
        {
            using var connection = context.CreateConnection();

            await connection.OpenAsync();

            var getUserByIdCommand = "SELECT " +
                "u.Id, u.CreatedAt," +
                "ul.UserNAme, ul.Email, ul.PasswordHash, ul.PasswordSalt " +
                "FROM UsersInfo AS u " +
                "INNER JOIN UsersLoginInfo AS ul ON u.Id = ul.UserId " +
                "WHERE u.Id = @id;";

            var user = await connection.QuerySingleOrDefaultAsync<User>(getUserByIdCommand, new { id });

            return Result<User?>.Success(user);
        }
        catch (Exception ex)
        {
            return Result<User?>.Failure(UserErrors.InternalError(ex.Message));
        }
    }
}
