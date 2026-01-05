using QuickTalk.Identity.Application.Interfaces;
using QuickTalk.Identity.Domain.Entities;
using QuickTalk.Identity.Persistence.Interfaces;

namespace QuickTalk.Identity.Application.Services;

public class UserService(
    IUserRepository userRepository,
    IPasswordService passwordService,
    TimeProvider timeProvider) : IUserService
{
    public User CreateUser(string login, string userName, string password)
    {
        var hash = passwordService.HashPassword(password);
        return User.Create(login, timeProvider.GetUtcNow().UtcDateTime, userName, hash.Password, hash.Salt);
    }

    public async Task<Result> SaveUserRelatedDataAsync(User user, RefreshToken token)
    {
        var searchOfExistingUserResult = await userRepository.GetUserByEmailAsync(user.Email);

        if (searchOfExistingUserResult.Data != null)
        {
            return Result.Failure(UserErrors.EmailAlreadyRegistered);
        }

        return await userRepository.CreateUserRelatedDataAsync(user, token);
    }

    public async Task<Result<User?>> GetUserByEmailAsync(string email) =>
        await userRepository.GetUserByEmailAsync(email);

    public async Task<Result<User?>> GetUserByIdAsync(Guid id) =>
        await userRepository.GetUserByIdAsync(id);

    public async Task<Result<User?>> GetAuthenticatedUserAsync(string email) =>
         await userRepository.GetUserByEmailAsync(email);
}
