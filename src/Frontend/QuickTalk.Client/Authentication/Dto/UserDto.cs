namespace QuickTalk.Client.Authentication.Dto;

public class UserDto(Guid id, string email, string userName)
{
    public Guid Id { get; private set; } = id;
    public string Email { get; private set; } = email;
    public string UserName { get; set; } = userName;
}
