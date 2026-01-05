using Hangfire;
using Hangfire.Redis.StackExchange;
using MassTransit;
using QuickTalk.Identity.Application.Models;
using QuickTalk.Identity.Application.Services;
using QuickTalk.Identity.Persistence;
using QuickTalk.Identity.WebApi.ConfigurationModels;
using QuickTalk.Identity.WebApi.ExceptionHandlers;
using QuickTalk.Identity.WebApi.ServiceCollectionExtensions;
using StackExchange.Redis;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        var dbSettings = new DataBaseConfiguration();
        builder.Configuration.GetSection("PostgresConnection").Bind(dbSettings);
        builder.Services.AddSingleton(dbSettings);

        var jwtConfig = new JwtConfig();
        builder.Configuration.GetSection("jwt").Bind(jwtConfig);
        builder.Services.AddSingleton(jwtConfig);

        var mqConfiguration = new RabbitMqHostConfiguration();
        builder.Configuration.GetSection("RabbitMq").Bind(mqConfiguration);
        builder.Services.AddSingleton(mqConfiguration);

        builder.Services.AddJtwAuthentication(jwtConfig);

        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<ExceptionHandler>();

        builder.Services.AddPersistenceDependencies();
        builder.Services.AddApplicationDependencies();

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
        });

        var registeredServices = builder.Services.ToList();

        builder.Services.AddHangfire(config =>
        {
            config.UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseRedisStorage(builder.Configuration.GetConnectionString("RedisConnection")!);
        });

        builder.Services.AddHangfireServer();

        builder.Services.AddSingleton<IConnectionMultiplexer>(x =>
            ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection")!));

        builder.Services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, config) =>
            {
                config.Host(mqConfiguration.Host, h =>
                {
                    h.Username(mqConfiguration.UserName);
                    h.Password(mqConfiguration.Password);
                });
            });
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseExceptionHandler();

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseCors();

        app.UseAuthorization();

        app.UseHangfireDashboard("/hangfire");

        app.MapControllers();

        RecurringJob.AddOrUpdate<RefreshTokenService>(
            "cleanUp-refreshToken-job",
            x => x.ExecuteCleanUpAsync(),
            Cron.Daily);

        app.Run();
    }
}
