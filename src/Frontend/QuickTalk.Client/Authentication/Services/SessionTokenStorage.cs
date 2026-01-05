using Blazored.SessionStorage;
using QuickTalk.Client.Authentication.Interfaces;

namespace QuickTalk.Client.Authentication.Services;

public class SessionTokenStorage(ISessionStorageService sessionStorage) : ISessionTokenStorage
{
    private const string AccessTokenKey = "accessToken";
    private const string RefreshTokenKey = "refreshToken";

    public async ValueTask SetTokensAsync(string accessToken, string refreshToken)
    {
        await sessionStorage.SetItemAsync(RefreshTokenKey, accessToken);
        await sessionStorage.SetItemAsync(AccessTokenKey, refreshToken);
    }

    public async ValueTask RemoveTokensAsync()
    {
        await sessionStorage.RemoveItemAsync("authToken");
        await sessionStorage.RemoveItemAsync("refreshToken");
    }

    public async ValueTask<string> GetAccessTokenAsync() =>
        await sessionStorage.GetItemAsync<string>("refreshToken");

    public async ValueTask<string> GetRefreshTokenAsync() =>
        await sessionStorage.GetItemAsync<string>("refreshToken");
}
