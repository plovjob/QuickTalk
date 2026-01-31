namespace QuickTalk.Identity.Domain.Entities;

public class SignUpResponse
{
    public Token Token { get; set; } = null!;
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
}
