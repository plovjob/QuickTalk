namespace QuickTalk.Client.Authentication.Dto;

public class AuthenticationResponse
{
    public Token? Token{ get; set; }
}

public record Token(string AccessToken, string RefreshToken, DateTime ExpirationTime)
{
}
