namespace QuickTalk.Client.Models;

internal sealed class Content(MessageDto messageDto)
{
    public MessageDto MessageDto { get; } = messageDto;
}
