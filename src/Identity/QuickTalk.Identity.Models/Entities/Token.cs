namespace QuickTalk.Identity.Domain.Entities;

public record Token(string AccessToken, string RefreshToken, DateTime ExpirationTime)
{
}
