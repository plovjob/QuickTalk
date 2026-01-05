namespace QuickTalk.Client.Authentication.Interfaces;

public interface ISessionTokenStorage
{
    ValueTask SetTokensAsync(string accessToken, string refreshToken);
    ValueTask RemoveTokensAsync();
    ValueTask<string> GetAccessTokenAsync();
    ValueTask<string> GetRefreshTokenAsync();
}
