using Blazored.SessionStorage;
using QuickTalk.Client.Authentication.Interfaces;

namespace QuickTalk.Client.Authentication.Services;

public class SessionTokenStorage(ISessionStorageService sessionStorage) : ISessionTokenStorage
{
    private const string AccessTokenKey = "accessToken";
    private const string RefreshTokenKey = "refreshToken";
    private const string UserNameKey = "userName";
    private const string UserIdKey = "userId";

    public async ValueTask SetTokensAsync(string accessToken, string refreshToken)
    {
        await sessionStorage.SetItemAsync(RefreshTokenKey, accessToken);
        await sessionStorage.SetItemAsync(AccessTokenKey, refreshToken);
    }

    public async ValueTask SetUserSensitiveInfoAsync(Guid userId, string userName, string accessToken, string refreshToken)
    {
        await sessionStorage.SetItemAsync(RefreshTokenKey, accessToken);
        await sessionStorage.SetItemAsync(AccessTokenKey, refreshToken);
        await sessionStorage.SetItemAsync(UserIdKey, userId);
        await sessionStorage.SetItemAsync(UserNameKey, userName);
    }

    public async ValueTask RemoveTokensAsync()
    {
        await sessionStorage.RemoveItemAsync("authToken");
        await sessionStorage.RemoveItemAsync("refreshToken");
        await sessionStorage.RemoveItemAsync("userName");
        await sessionStorage.RemoveItemAsync("userId");
    }

    public async ValueTask<string> GetAccessTokenAsync() =>
        await sessionStorage.GetItemAsync<string>("refreshToken");

    public async ValueTask<Guid> GetCurrentUserIdAsync() =>
        await sessionStorage.GetItemAsync<Guid>("userId");

    public async ValueTask<Guid> GetCurrentUserNameAsync() =>
        await sessionStorage.GetItemAsync<Guid>("userName");

    public async ValueTask<string> GetRefreshTokenAsync() =>
        await sessionStorage.GetItemAsync<string>("refreshToken");
}
