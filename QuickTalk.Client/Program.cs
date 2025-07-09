using QuickTalk.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ChatMessanger.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Configuration.AddJsonFile(Path.Combine());
var apiBaseAddress = "https://localhost:7005";
builder.Services.AddScoped(sp
    => new HttpClient { BaseAddress = new Uri(apiBaseAddress) });
builder.Services.AddScoped<ApiMessageService>();
await builder.Build().RunAsync();
