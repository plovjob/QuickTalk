using System.Globalization;
using System.Security.Claims;
using QuickTalk.Identity.Application.Interfaces;
using QuickTalk.Identity.Application.Models;
using QuickTalk.Identity.Domain.Entities;

namespace QuickTalk.Identity.Application.Services;

public class AuthenticationService(
    IUserService userService,
    ITokenService tokenService,
    JwtConfig jwtConfig,
    TimeProvider timeProvider,
    IPasswordService passwordService,
    IRefreshTokenService refreshTokenService) : IAuthenticationService
{
    public async Task<Result<Token?>> SignInAsync(string login, string password)
    {
        if (string.IsNullOrWhiteSpace(login) ||
          string.IsNullOrWhiteSpace(password))
        {
            return Result<Token?>.Failure(UserErrors.InvalidLogin);
        }

        var searchExistingUserResult = await userService.GetAuthenticatedUserAsync(login);

        if (searchExistingUserResult.IsFailure)
        {
            return Result<Token?>.Failure(searchExistingUserResult.Error!);
        }

        if (searchExistingUserResult.Data is null)
        {
            return Result<Token?>.Failure(UserErrors.UserNotFound(login));
        }

        var user = searchExistingUserResult.Data;

        var isPasswordVerified = passwordService.VerifyPassword(password, user!.PasswordHash, user!.PasswordSalt);

        if (!isPasswordVerified)
        {
            return Result<Token?>.Failure(UserErrors.InvalidLogin);
        }

        var claims = BuildClaims(user!);
        var refreshToken = BuildUserRefreshToken(user.Id);
        var token = tokenService.CreateToken(claims);

        var tokenObject = new Token(
            token,
            refreshToken.Token,
            tokenService.GetTokenExpiration(token));

        var addOrUpdateRefreshTokenResult = await refreshTokenService.UpdateRefreshTokenAsync(refreshToken);

        if (!addOrUpdateRefreshTokenResult.IsSuccess)
        {
            return Result<Token?>.Failure(addOrUpdateRefreshTokenResult.Error!);
        }

        return Result<Token?>.Success(tokenObject);
    }

    public async Task<Result<Token?>> SignUpAsync(string login, string userName, string password)
    {
        if (string.IsNullOrWhiteSpace(login) ||
          string.IsNullOrWhiteSpace(password))
        {
            return Result<Token?>.Failure(UserErrors.InvalidLogin);
        }

        var user = userService.CreateUser(login, userName, password);

        var claims = BuildClaims(user!);
        var accessToken = tokenService.CreateToken(claims);
        var refreshToken = BuildUserRefreshToken(user!.Id);

        var userDataSavingResult = await userService.SaveUserRelatedDataAsync(user, refreshToken);

        if (userDataSavingResult.IsFailure)
        {
            return Result<Token?>.Failure(userDataSavingResult.Error!);
        }

        var identityToken = new Token(
            accessToken,
            refreshToken.Token,
            tokenService.GetTokenExpiration(accessToken));

        return Result<Token?>.Success(identityToken);
    }

    public async Task<Result<Token?>> RefreshTokenAsync(string refreshToken)
    {
        var getRefreshTokenObjectResult = await refreshTokenService.GetByRefreshTokenAsync(refreshToken);

        if (getRefreshTokenObjectResult.IsFailure && getRefreshTokenObjectResult.Data != null)
        {
            return Result<Token?>.Failure(UserErrors.InvalidRefreshToken);
        }

        var userRefreshToken = getRefreshTokenObjectResult.Data;

        if (userRefreshToken == null || userRefreshToken.Token != refreshToken || userRefreshToken.ExpiresAt <= timeProvider.GetUtcNow().UtcDateTime)
        {
            return Result<Token?>.Failure(UserErrors.InvalidRefreshToken);
        }

        var userId = userRefreshToken.UserId;

        var getUserResult = await userService.GetUserByIdAsync(userId);

        if (getUserResult.IsFailure)
        {
            return Result<Token?>.Failure(getUserResult.Error!);
        }

        var user = getUserResult.Data;

        if (user == null)
        {
            return Result<Token?>.Failure(UserErrors.UserNotFound(userId));
        }

        var claims = BuildClaims(user);
        var newJwtToken = tokenService.CreateToken(claims);

        var newRefreshToken = BuildUserRefreshToken(userId);

        var updatingResult = await refreshTokenService.UpdateRefreshTokenAsync(newRefreshToken);

        if (updatingResult.IsFailure)
        {
            return Result<Token?>.Failure(updatingResult.Error!);
        }

        var token = new Token(
           newJwtToken,
           newRefreshToken.Token,
           tokenService.GetTokenExpiration(newJwtToken));

        return Result<Token?>.Success(token);
    }

    private Claim[] BuildClaims(User user)
    {
        return new[]
        {
            new Claim("id", user.Id.ToString()),
            new Claim("login", user.Email),
            new Claim("hash", user.PasswordHash),
            new Claim("salt", user.PasswordSalt),
            new Claim("createdAt", user.CreatedAt.ToString(CultureInfo.InvariantCulture)),
            new Claim("userName", user.UserName)
        };
    }

    private RefreshToken BuildUserRefreshToken(Guid id)
    {
        return new RefreshToken(
            id,
            tokenService.BuildRefreshToken(),
            timeProvider.GetUtcNow().UtcDateTime,
            timeProvider.GetUtcNow().AddDays(jwtConfig.RefreshTokenExpiration).UtcDateTime);
    }
}
