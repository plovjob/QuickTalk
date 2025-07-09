namespace QuickTalk.Messages.Domain.Entities;

public class Message(string userName, string text, DateTime sentAt)
{
    public Guid Id { get; } = Guid.NewGuid();
    public string UserName { get; } = userName;
    public string Text { get; } = text;
    public DateTime SentAt { get; } = sentAt;
}
