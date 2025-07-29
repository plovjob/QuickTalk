namespace QuickTalk.Messages.Domain.Dto;

public sealed class MessageDto(string userName, string text, DateTime? sentAt)
{
    public string UserName { get; } = userName;
    public string Text { get; } = text;
    public DateTime? SentAt { get; set; } = sentAt;
}
