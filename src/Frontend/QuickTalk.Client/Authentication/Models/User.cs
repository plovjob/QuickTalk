using System.Globalization;
using System.Security.Claims;

namespace QuickTalk.Client.Authentication.Models;

public class User(Guid id, DateTime createdAt, string email, string passwordHash, string passwordSalt, string userName)
{
    public Guid Id { get; private set; } = id;
    public string Email { get; private set; } = email;
    public DateTime CreatedAt { get; private set; } = createdAt;
    public string UserName { get; set; } = userName;

    public string PasswordHash { get; set; } = passwordHash;
    public string PasswordSalt { get; set; } = passwordSalt;

    public static ClaimsPrincipal ToClaimsPrincipal(User user)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new("id", user.Id.ToString()),
            new("createdAt", user.CreatedAt.ToString(CultureInfo.InvariantCulture)),
            new("login", user.Email),
            new("hash", user.PasswordHash),
            new("salt", user.PasswordSalt)
        }, "Bearer"));
    }

    public static User FromClaimsPrincipal(ClaimsPrincipal claimsPrincipal) => new(
        id: new Guid(claimsPrincipal.FindFirst("id")!.Value),
        createdAt: DateTime.Parse(claimsPrincipal.FindFirst("createdAt")!.Value, CultureInfo.InvariantCulture),
        email: claimsPrincipal.FindFirst("login")!.Value,
        passwordHash: claimsPrincipal.FindFirst("hash")!.Value,
        passwordSalt: claimsPrincipal.FindFirst("salt")!.Value,
        userName: claimsPrincipal.FindFirst("userName")!.Value);
}
