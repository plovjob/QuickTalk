namespace QuickTalk.Client.Models;

internal sealed class MessageDto(Guid? Id, string? userName, string? text)
{
    public Guid Id { get; set; } = Id ?? throw new ArgumentNullException();
    public string UserName { get; } = userName ?? throw new ArgumentNullException();
    public string Text { get; } = text ?? throw new ArgumentNullException();
}
