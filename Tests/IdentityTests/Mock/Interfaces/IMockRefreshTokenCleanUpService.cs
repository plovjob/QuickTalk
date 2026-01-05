namespace QuickTalk.Identity.ComponentTests.Mock.Interfaces;

public interface IMockRefreshTokenCleanUpService
{
    Task ClearExpiredTokensAsync();
}
