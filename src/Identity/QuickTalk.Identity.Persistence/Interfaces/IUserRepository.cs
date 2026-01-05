using QuickTalk.Identity.Domain.Entities;

namespace QuickTalk.Identity.Persistence.Interfaces;

public interface IUserRepository
{
    Task<Result> CreateUserRelatedDataAsync(User user, RefreshToken token);
    Task<Result<User?>> GetUserByEmailAsync(string email);
    Task<Result<User?>> GetUserByIdAsync(Guid id);
}
