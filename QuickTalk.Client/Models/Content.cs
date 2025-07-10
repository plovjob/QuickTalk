namespace QuickTalk.Client.Models;

public class Content
{
    public MessageDto? Message{ get; set; }
}

public class MessageDto
{
    public string? UserName { get; set; } = null!;
    public string? Text { get; set; } = null!;
}
