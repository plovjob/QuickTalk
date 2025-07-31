using QuickTalk.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using QuickTalk.Client.Services;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

var baseUrl = builder.Configuration.GetValue<string>("ApiSettings:BaseUrl") ?? "";
builder.Services.Configure<ApiBaseAddressConfiguration>(e =>
{
    e.BaseAddress = baseUrl;
});

 builder.Services.AddScoped(sp
    => new HttpClient { BaseAddress = new Uri(baseUrl) });

builder.Services.AddScoped<ApiMessageService>();
builder.Services.AddMudServices();
await builder.Build().RunAsync();
