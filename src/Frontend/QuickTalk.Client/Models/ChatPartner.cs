namespace QuickTalk.Client.Models;

public class ChatPartner(Guid id, string name)
{
    public Guid Id { get; set; } = id;
    public string Name { get; set; } = name;
}
