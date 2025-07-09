namespace QuickTalk.Messages.Domain.Dto;

public class MessageDto(string UserName, string Text)
{
    public string UserName { get; } = UserName;
    public string Text { get; } = Text;
}
