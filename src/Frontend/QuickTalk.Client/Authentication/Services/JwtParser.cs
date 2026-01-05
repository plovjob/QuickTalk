using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace QuickTalk.Client.Authentication.Services;

public static class JwtParser
{
    public static IEnumerable<Claim> GetClaimsFromJwt(string? jwt)
    {
        if (string.IsNullOrEmpty(jwt))
        {
            return Enumerable.Empty<Claim>();
        }

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(jwt);
        return jwtToken.Claims;
    }
}
