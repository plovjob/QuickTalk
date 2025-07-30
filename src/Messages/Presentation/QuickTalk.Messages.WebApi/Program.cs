using System.Globalization;
using QuickTalk.Messages.WebApi.Extensions;
using QuickTalk.Messages.WebApi.Hubs;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console(formatProvider: CultureInfo.CurrentCulture, theme: AnsiConsoleTheme.Code)
    .CreateLogger();
builder.Services.AddSerilog();
#region other_di

builder.Services.AddPersistenceDependencies(builder.Configuration);
builder.Services.AddApplicationDependencies();
builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddCors();

#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors(builder =>
{
    builder.AllowAnyHeader();
    builder.AllowAnyMethod();
    builder.AllowAnyOrigin();
});

app.UseAuthorization();
app.MapHub<MessageHub>("/chathub");
app.MapControllers();
app.Run();
