namespace QuickTalk.Client.Models;

public class MessageDTO
{
    public string UserName { get; set; } = null!;
    public string Text { get; set; } = null!;
    public DateTime TimeOfSend { get; set; }
}
