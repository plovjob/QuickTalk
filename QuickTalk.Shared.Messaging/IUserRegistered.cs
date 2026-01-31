namespace QuickTalk.Shared.Messaging;

public interface IUserRegistered
{
    public Guid Id { get; set; }
    public string UserName { get; }
}
