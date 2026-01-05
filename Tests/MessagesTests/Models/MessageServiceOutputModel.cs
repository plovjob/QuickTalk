namespace QuickTalk.Messages.ComponentTests.Models;

class MessageServiceOutputModel(Guid id, string userName, string text, DateTime sentAt, DateTime? editedAt)
{
    public Guid Id { get; private set; } = id;
    public string UserName { get; private set; } = userName;
    public string Text { get; private set; } = text;
    public DateTime SentAt { get; private set; } = sentAt;
    public DateTime? EditedAt { get; private set; } = editedAt;
}
