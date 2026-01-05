using QuickTalk.Identity.Domain.Entities;

namespace QuickTalk.Identity.Application.Interfaces;

public interface IUserService
{
    User CreateUser(string login, string userName, string password);
    Task<Result> SaveUserRelatedDataAsync(User user, RefreshToken token);
    Task<Result<User?>> GetUserByEmailAsync(string email);
    Task<Result<User?>> GetUserByIdAsync(Guid id);
    Task<Result<User?>> GetAuthenticatedUserAsync(string email);
}
