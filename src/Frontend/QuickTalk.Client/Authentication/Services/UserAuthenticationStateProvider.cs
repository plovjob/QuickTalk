using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using QuickTalk.Client.Authentication.Interfaces;

namespace QuickTalk.Client.Authentication.Services;

public class UserAuthenticationStateProvider(ISessionTokenStorage tokenStorage) : AuthenticationStateProvider
{
    private ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await tokenStorage.GetAccessTokenAsync();

        if (string.IsNullOrEmpty(token))
        {
            return new AuthenticationState(_anonymous);
        }

        var identity = new ClaimsIdentity(JwtParser.GetClaimsFromJwt(token), "jwt");

        var user = new ClaimsPrincipal(identity);

        return await Task.FromResult(new AuthenticationState(user));
    }

    public void AuthenticateUser(string token)
    {
        var identity = new ClaimsIdentity(JwtParser.GetClaimsFromJwt(token), "jwt");

        var user = new ClaimsPrincipal(identity);

        var state = new AuthenticationState(user);

        NotifyAuthenticationStateChanged(Task.FromResult(state));
    }

    public void NotifyUserLogOut()
    {
        var authenticationState = Task.FromResult(new AuthenticationState(_anonymous));
        NotifyAuthenticationStateChanged(authenticationState);
    }
}
