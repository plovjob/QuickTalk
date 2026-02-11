namespace QuickTalk.Client.Authentication.Dto;

public class UserDto(Guid id, string userName)
{
    public Guid Id { get; private set; } = id;
    public string UserName { get; set; } = userName;
}
