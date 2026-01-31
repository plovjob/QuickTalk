namespace QuickTalk.Client.Authentication.Interfaces;

public interface ISessionTokenStorage
{
    ValueTask SetTokensAsync(string accessToken, string refreshToken);
    ValueTask SetUserSensitiveInfoAsync(Guid userId, string userName, string accessToken, string refreshToken);
    ValueTask RemoveTokensAsync();
    ValueTask<string> GetAccessTokenAsync();
    ValueTask<string> GetRefreshTokenAsync();
    ValueTask<Guid> GetCurrentUserIdAsync();
    ValueTask<string> GetCurrentUserNameAsync();
}
