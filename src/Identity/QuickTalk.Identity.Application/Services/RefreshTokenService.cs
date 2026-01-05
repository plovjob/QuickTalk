using Hangfire;
using QuickTalk.Identity.Application.Interfaces;
using QuickTalk.Identity.Domain.Entities;
using QuickTalk.Identity.Persistence.Interfaces;

namespace QuickTalk.Identity.Application.Services;

public class RefreshTokenService(IRefreshTokenRepository refreshTokenRepository) : IRefreshTokenService
{
    public async Task<Result> AddUserRefreshTokenAsync(RefreshToken token) =>
        await refreshTokenRepository.AddUserRefreshTokenAsync(token);

    [DisableConcurrentExecution(timeoutInSeconds: 300)]
    public async Task ExecuteCleanUpAsync()
    {
        await refreshTokenRepository.ClearExpiredTokensAsync();
    }

    public async Task<Result<RefreshToken?>> GetByRefreshTokenAsync(string refreshToken) =>
        await refreshTokenRepository.GetByTokenAsync(refreshToken);

    public async Task<Result<RefreshToken?>> GetByUserIdAsync(Guid id) =>
        await refreshTokenRepository.GetByUserIdAsync(id);

    public async Task<Result> UpdateRefreshTokenAsync(RefreshToken token) =>
        await refreshTokenRepository.UpdateRefreshTokenAsync(token);
}
