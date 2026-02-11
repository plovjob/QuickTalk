namespace QuickTalk.Client.Models;

internal sealed class MessageDto(Guid? messageId, Guid fromUserId, Guid toUserId, string? text, DateTime sentAt, DateTime editedAt)
{
    public Guid Id { get; set; } = messageId ?? throw new ArgumentNullException();
    public Guid FromUserId { get; set; } = fromUserId;
    public Guid ToUserID { get; set; } = toUserId;
    public string Text { get; } = text ?? throw new ArgumentNullException();
    public DateTime SentAt { get; set; } = sentAt;
    public DateTime? EditedAt { get; set; } = editedAt;
}
