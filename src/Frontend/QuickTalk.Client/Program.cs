using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using QuickTalk.Client;
using QuickTalk.Client.Authentication.Handlers;
using QuickTalk.Client.Authentication.Interfaces;
using QuickTalk.Client.Authentication.Services;
using QuickTalk.Client.Interfaces;
using QuickTalk.Client.ServiceCollectionExtensions;
using QuickTalk.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

var baseAddresses = new ApiBaseAddressConfiguration();
builder.Configuration.GetSection("ApiSettings").Bind(baseAddresses);
builder.Services.AddSingleton(baseAddresses);

builder.Services.AddTransient<AuthenticationHeaderHttpInterceptor>();
builder.Services.AddTransient<RequestRetryHttpInterceptor>();

builder.Services.AddHttpClientConfiguration(baseAddresses);

builder.Services.AddScoped<ChatManager>();
builder.Services.AddMudServices();

builder.Services.AddTransient<IChatManager, ChatManager>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ISessionTokenStorage, SessionTokenStorage>();

builder.Services.AddBlazoredSessionStorageAsSingleton();
builder.Services.AddScoped<UserAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
{
    using var scope = sp.CreateScope();
    return scope.ServiceProvider.GetRequiredService<UserAuthenticationStateProvider>();
});

builder.Services.AddScoped<HubService>();

builder.Services.AddSingleton<TestService>();
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();
