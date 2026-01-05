namespace QuickTalk.Client.Authentication.Dto;

public class RefreshToken
{
    public string JwtToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}
