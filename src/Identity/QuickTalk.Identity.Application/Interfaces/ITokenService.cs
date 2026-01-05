using System.Security.Claims;

namespace QuickTalk.Identity.Application.Interfaces;

public interface ITokenService
{
    string CreateToken(Claim[] claims);
    string BuildRefreshToken();
    ClaimsPrincipal? GetPrincipalFromToken(string token);
    DateTime GetTokenExpiration(string token);
}
