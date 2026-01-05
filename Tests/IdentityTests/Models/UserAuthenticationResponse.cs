namespace QuickTalk.Identity.ComponentTests.Models;

public class UserAuthenticationResponse
{
    public Token Token { get; set; } = null!;
}

public record Token(string AccessToken, string RefreshToken, DateTime ExpirationTime)
{
}

//public class RefreshToken(Guid userId, string token, DateTime issuedAt, DateTime expiresAt)
//{
//    public string Token { get; set; } = token;
//    public Guid UserId { get; set; } = userId;
//    public DateTime IssuedAt { get; set; } = issuedAt;
//    public DateTime ExpiresAt { get; set; } = expiresAt;
//}
