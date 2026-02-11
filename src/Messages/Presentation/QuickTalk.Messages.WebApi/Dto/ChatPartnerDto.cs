namespace QuickTalk.Messages.WebApi.Dto;

public class ChatPartnerDto(Guid id, string name)
{
    public Guid Id { get; set; } = id;
    public string Name { get; set; } = name;
}
