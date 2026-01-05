using QuickTalk.Identity.Domain.Entities;

namespace QuickTalk.Identity.Persistence.Interfaces;

public interface IRefreshTokenRepository
{
    Task<Result> AddUserRefreshTokenAsync(RefreshToken token);
    Task<Result> UpdateRefreshTokenAsync(RefreshToken token);
    Task<Result<RefreshToken?>> GetByUserIdAsync(Guid id);
    Task ClearExpiredTokensAsync();
    Task<Result<RefreshToken?>> GetByTokenAsync(string refreshToken);
}
