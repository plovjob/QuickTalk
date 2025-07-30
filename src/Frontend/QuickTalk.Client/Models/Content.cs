namespace QuickTalk.Client.Models;

internal sealed class MessageDto(string? userName, string? text, DateTime? sentAt = null)
{
    public string? UserName { get; } = userName ?? throw new ArgumentNullException();
    public string? Text { get; } = text ?? throw new ArgumentNullException();
    public DateTime? SentAt { get; } = sentAt;
}

internal sealed class Content(MessageDto message)
{
    public MessageDto Message { get; } = message;
}
