using System.Globalization;
using MassTransit;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using QuickTalk.Messages.Domain.Entities;
using QuickTalk.Messages.Persistence;
using QuickTalk.Messages.WebApi.ConfigurationModels;
using QuickTalk.Messages.WebApi.Consumers;
using QuickTalk.Messages.WebApi.Extensions;
using QuickTalk.Messages.WebApi.Hubs;
using QuickTalk.Messages.WebApi.ServiceCollectionExtensions;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddHttpContextAccessor();
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.Console(formatProvider: CultureInfo.CurrentCulture, theme: AnsiConsoleTheme.Code)
            .CreateLogger();
        builder.Services.AddSerilog();

        var environment = builder.Environment;
        builder.Services.AddPersistenceDependencies(builder.Configuration.GetConnectionString("PostgresSqlConnection")!);
        builder.Services.AddSingleton(TimeProvider.System);

        builder.Services.AddApplicationDependencies();

        builder.Services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
        });

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        builder.Services.AddCors();

        var jwtConfig = new JwtConfig();
        builder.Configuration.GetSection("jwt").Bind(jwtConfig);
        builder.Services.AddSingleton(jwtConfig);

        var mqConfiguration = new RabbitMqHostConfiguration();
        builder.Configuration.GetSection("RabbitMq").Bind(mqConfiguration);
        builder.Services.AddSingleton(mqConfiguration);

        builder.Services.AddJtwAuthentication(jwtConfig);

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowBlazorClient", builder =>
            {
                builder.WithOrigins("https://localhost:7213")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        builder.Services.AddMassTransit(x =>
        {
            x.AddConsumer<PublisherService>();

            x.UsingRabbitMq((context, config) =>
            {
                config.Host(mqConfiguration.Host, config =>
                {
                    config.Username(mqConfiguration.UserName);
                    config.Password(mqConfiguration.Password);
                });

                config.ReceiveEndpoint("hello-message-event", e =>
                {
                    e.Consumer<PublisherService>();
                });
            });
        });

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService("Message Service"))
            .WithTracing(config =>
            {
                config.AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSource()
                .AddOtlpExporter(options =>
                {

                });
            });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowBlazorClient");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapHub<MessageHub>("/chathub");
        app.MapControllers();
        app.Run();
    }
}
