using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using QuickTalk.Messages.Domain.Entities;

namespace QuickTalk.Messages.WebApi.ServiceCollectionExtensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddJtwAuthentication(
        this IServiceCollection services,
        JwtConfig jwtConfig)
    {
        services.AddAuthorization();
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.RequireHttpsMetadata = true;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
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

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var authorizationHeader = context.Request.Headers.Authorization.ToString();
                    context.Token = authorizationHeader.Length > 0 ? context.Request.Headers.Authorization.ToString().Substring(7).Trim() : authorizationHeader;

                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }
}
