using QuickTalk.Client.Authentication.Models;

namespace QuickTalk.Client.Interfaces;

public interface IUserService
{
    User GetCurrentUser();
    List<User> GetAllRegisteredUsers();
}
