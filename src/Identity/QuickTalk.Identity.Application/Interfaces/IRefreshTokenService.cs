using QuickTalk.Identity.Domain.Entities;

namespace QuickTalk.Identity.Application.Interfaces;

public interface IRefreshTokenService
{
    Task ExecuteCleanUpAsync();
    Task<Result<RefreshToken?>> GetByUserIdAsync(Guid id);
    Task<Result<RefreshToken?>> GetByRefreshTokenAsync(string refreshToken);
    Task<Result> AddUserRefreshTokenAsync(RefreshToken token);
    Task<Result> UpdateRefreshTokenAsync(RefreshToken token);
}
