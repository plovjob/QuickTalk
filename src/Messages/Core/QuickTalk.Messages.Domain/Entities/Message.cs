namespace QuickTalk.Messages.Domain.Entities;

public sealed class Message(Guid id, string text)
{
    private DateTime? _sentAt = null;

    public Guid Id { get; private set; } = id;

    public string Text { get; private set; } = text;

    public DateTime? SentAt
    {
        get => _sentAt;
        private set
        {
            if (!value.HasValue)
            {
                throw new ArgumentException("Message sending time cannot be null");
            }

            if (value.Value.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("DateTime must have UTC kind");
            }

            _sentAt = value.Value;
        }
    }

    public DateTime? EditedAt { get; private set; } = null;
    public Guid FromUserId { get; private set; }
    public Guid ToUserId { get; private set; }
    public MessengerUser? FromUser { get; set; }
    public MessengerUser? ToUser { get; set; }

    public void UpdateText(string text) => Text = text;

    public void SetSentAt(DateTime sentAt)
    {
        if (SentAt == null)
        {
            SentAt = sentAt;
        }
    }

    public void UpdateEditedAt(DateTime editedAt) => EditedAt = editedAt;

    public void ResetEditedAt() => EditedAt = null;
}
