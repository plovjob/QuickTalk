using System.Net;
using System.Net.Http.Headers;
using Polly;
using QuickTalk.Client.Authentication.Interfaces;

namespace QuickTalk.Client.Authentication.Handlers;

public class AuthenticationHeaderHttpInterceptor(
    IAuthenticationService authenticationService,
    ISessionTokenStorage tokenStorage)
    : DelegatingHandler
{
    private string accessToken = "";

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        accessToken = await tokenStorage.GetAccessTokenAsync();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var unauthorizedPolicy = Policy
            .Handle<HttpRequestException>(ex => ex.StatusCode == HttpStatusCode.Unauthorized)
            .RetryAsync(async (exception, retryCount) =>
            {
                var refreshToken = await tokenStorage.GetRefreshTokenAsync();
                var newAccessToken = await authenticationService.RefreshTokenAsync(refreshToken);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newAccessToken);
            });

        var policyResult = await unauthorizedPolicy.ExecuteAndCaptureAsync(() => base.SendAsync(request, cancellationToken));

        return policyResult.Result;
    }
}
