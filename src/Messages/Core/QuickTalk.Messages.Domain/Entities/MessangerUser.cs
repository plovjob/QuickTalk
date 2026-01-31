namespace QuickTalk.Messages.Domain.Entities;

public class MessangerUser(Guid id, string userName)
{
    public Guid Id { get; private set; } = id;
    public string UserName { get; private set; } = userName;
    public virtual ICollection<Message>? MessagesFromUsers { get; set; } = new HashSet<Message>();
    public virtual ICollection<Message>? MessagesToUsers { get; set; } = new HashSet<Message>();
}
