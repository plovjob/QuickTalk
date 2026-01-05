using System.Net.Http.Json;
using QuickTalk.Client.Authentication.Dto;
using QuickTalk.Client.Authentication.Interfaces;

namespace QuickTalk.Client.Authentication.Services;

public class AuthenticationService(
    IHttpClientFactory factory,
    ISessionTokenStorage tokenStorage) : IAuthenticationService
{
    public async Task<AuthenticationResponse> LoginAsync(LoginRequest request)
    {
        var response = await factory.CreateClient("IdentityService").PostAsync(
            "api/account/signin",
            JsonContent.Create(request));

        if (!response.IsSuccessStatusCode)
        {
            throw new UnauthorizedAccessException("Login failed.");
        }

        var content = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();

        if (content == null)
        {
            throw new InvalidDataException();
        }

        await tokenStorage.SetTokensAsync(content.Token!.AccessToken, content.Token!.RefreshToken);

        return content;
    }

    public async Task<AuthenticationResponse> RegisterAsync(RegistrationRequest request)
    {
        var client = factory.CreateClient("IdentityService");

        var response = await client.PostAsync(
            "api/account/signup",
            JsonContent.Create(request));

        if (!response.IsSuccessStatusCode)
        {
            //var errorModel = await response.Content.ReadFromJsonAsync<>();
            throw new UnauthorizedAccessException("Registration failed.");
        }

        var content = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();

        if (content == null)
        {
            throw new InvalidDataException();
        }

        await tokenStorage.SetTokensAsync(content.Token!.AccessToken, content.Token!.RefreshToken);

        return content;
    }

    public async Task LogOutAsync()
    {
        await tokenStorage.RemoveTokensAsync();
    }

    public async Task<string> RefreshTokenAsync(string refreshToken)
    {
        var refreshTokenDto = new RefreshTokenDto { RefreshToken = refreshToken };

        var client = factory.CreateClient("IdentityService");

        var refreshResponse = await client.PostAsync("api/account/refreshtoken", JsonContent.Create(refreshTokenDto));

        if (!refreshResponse.IsSuccessStatusCode)
        {
            throw new ApplicationException("Something went wrong during the refresh token action");
        }

        var content = await refreshResponse.Content.ReadFromJsonAsync<AuthenticationResponse>();

        await tokenStorage.SetTokensAsync(content!.Token!.AccessToken, content.Token!.RefreshToken);

        return content!.Token!.AccessToken;
    }
}
