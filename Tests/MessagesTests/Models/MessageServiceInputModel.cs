namespace QuickTalk.Messages.ComponentTests.Models;

class MessageServiceInputModel(Guid id, string userName, string text)
{
    public Guid Id { get; private set; } = id;
    public string UserName { get; private set; } = userName;
    public string Text { get; private set; } = text;
}
