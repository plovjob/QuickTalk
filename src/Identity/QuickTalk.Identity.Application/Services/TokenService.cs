using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using QuickTalk.Identity.Application.Interfaces;
using QuickTalk.Identity.Application.Models;

namespace QuickTalk.Identity.Application.Services;

public class TokenService(JwtConfig jwtConfig, TimeProvider timeProvider) : ITokenService
{
    public string CreateToken(Claim[] claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
                    issuer: jwtConfig.Issuer,
                    audience: jwtConfig.Audience,
                    notBefore: timeProvider.GetUtcNow().UtcDateTime,
                    claims: claims,
                    expires: timeProvider.GetUtcNow().UtcDateTime.AddMinutes(jwtConfig.AccessTokenExpiration),
                    signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string BuildRefreshToken()
    {
        var randomNumber = new byte[32];
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal? GetPrincipalFromToken(string token)
    {
        var tokenValidator = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret));

        var parameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = jwtConfig.Audience,
            ValidateIssuer = true,
            ValidIssuer = jwtConfig.Issuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateLifetime = false
        };

        var principal = tokenValidator.ValidateToken(token, parameters, out var securityToken);

        if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return principal;
    }

    public DateTime GetTokenExpiration(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        var tokenExp = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")!.Value;
        var ticks = long.Parse(tokenExp, CultureInfo.InvariantCulture);

        return DateTimeOffset.FromUnixTimeSeconds(ticks).UtcDateTime;
    }
}
