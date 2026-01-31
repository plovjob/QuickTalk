namespace QuickTalk.Client.Models;

internal sealed class MessageDto(Guid? messageID, Guid fromUserId, Guid toUserId, string? userName, string? text)
{
    public Guid MessageId { get; set; } = messageID ?? throw new ArgumentNullException();
    public Guid FromUserId { get; set; } = fromUserId;
    public Guid ToUserID { get; set; } = toUserId;
    public string UserName { get; } = userName ?? throw new ArgumentNullException();
    public string Text { get; } = text ?? throw new ArgumentNullException();
}
