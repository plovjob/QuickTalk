namespace QuickTalk.Client.Authentication.Dto;

public class AuthenticationResponse
{
    public Token? Token{ get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
}

public record Token(string AccessToken, string RefreshToken, DateTime ExpirationTime)
{
}
