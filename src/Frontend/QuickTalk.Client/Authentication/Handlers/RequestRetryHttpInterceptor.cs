using Polly;
using Polly.Extensions.Http;

namespace QuickTalk.Client.Authentication.Handlers;

public class RequestRetryHttpInterceptor : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        var policyResult = await retryPolicy.ExecuteAndCaptureAsync(async () => await base.SendAsync(request, cancellationToken));

        if (policyResult.Outcome == OutcomeType.Failure)
        {
            return policyResult.FinalHandledResult;
        }

        return policyResult.Result;
    }
}
