using QuickTalk.Client.Authentication.Handlers;

namespace QuickTalk.Client.ServiceCollectionExtensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddHttpClientConfiguration(this IServiceCollection services, ApiBaseAddressConfiguration addressConfiguration)
    {
        services.AddHttpClient("WebApi", (provider, client) =>
        {
            client.BaseAddress = new Uri(addressConfiguration.MessagesWebApiUrl!);
        })
        .AddHttpMessageHandler<AuthenticationHeaderHttpInterceptor>()
        .AddHttpMessageHandler<RequestRetryHttpInterceptor>();

        services.AddHttpClient("IdentityService", (provider, client) =>
        {
            client.BaseAddress = new Uri(addressConfiguration.IdentityServiceUrl!);
        }).AddHttpMessageHandler<RequestRetryHttpInterceptor>();

        return services;
    }
}
