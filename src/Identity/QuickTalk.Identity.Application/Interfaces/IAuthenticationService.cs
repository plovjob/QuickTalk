using QuickTalk.Identity.Domain.Entities;

namespace QuickTalk.Identity.Application.Interfaces;

public interface IAuthenticationService
{
    Task<Result<Token?>> SignInAsync(string login, string password);
    Task<Result<Token?>> SignUpAsync(string login, string userName, string password);
    Task<Result<Token?>> RefreshTokenAsync(string refreshToken);
}
