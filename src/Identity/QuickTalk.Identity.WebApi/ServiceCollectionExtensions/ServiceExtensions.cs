using System.IO.Abstractions;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using QuickTalk.Identity.Application.Interfaces;
using QuickTalk.Identity.Application.Models;
using QuickTalk.Identity.Application.Services;
using QuickTalk.Identity.Persistence.DbContext;
using QuickTalk.Identity.Persistence.Interfaces;
using QuickTalk.Identity.Persistence.Migrations;
using QuickTalk.Identity.Persistence.Repository;

namespace QuickTalk.Identity.WebApi.ServiceCollectionExtensions;

internal static class ServiceExtensions
{
    internal static IServiceCollection AddPersistenceDependencies(this IServiceCollection services)
    {
        services.AddSingleton<IFileSystem, FileSystem>();
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<UserDbContext>();
        services.AddHostedService<MigrationRunner>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    internal static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddTransient<IPasswordService, PasswordService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        return services;
    }

    internal static IServiceCollection AddJtwAuthentication(this IServiceCollection services, JwtConfig jwtConfig)
    {
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = true;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtConfig.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtConfig.Audience,
                ValidateIssuerSigningKey = true,
                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig.Secret))
            };
        });

        return services;
    }
}
