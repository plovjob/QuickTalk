using System.Data.Common;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuickTalk.Identity.ComponentTests.Infrastructure;
using QuickTalk.Identity.ComponentTests.Mock.Interfaces;
using QuickTalk.Identity.ComponentTests.Mock.Services;
using QuickTalk.Identity.ComponentTests.Models;
using QuickTalk.Identity.ComponentTests.Services;
using QuickTalk.Identity.Persistence.DbContext;
using Respawn;

namespace QuickTalk.Identity.ComponentTests;

public class IdentityServiceComponentTests
    : IClassFixture<IdentityServiceWebApplicationFactory>, IAsyncLifetime
{
    private readonly IdentityServiceWebApplicationFactory _factory;
    private HttpClient httpClient = null!;
    private Respawner respawner = null!;
    private DbConnection connection = null!;

    public IdentityServiceComponentTests(IdentityServiceWebApplicationFactory factory)
    {
        _factory = factory;
        httpClient = _factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        connection = (DbConnection)scope.ServiceProvider.GetRequiredService<UserDbContext>().CreateConnection();
        connection.Open();
        respawner = factory.Services.GetServices<IHostedService>().OfType<RespawnerHostedService>().FirstOrDefault()!.Respawner;
    }

    public async Task InitializeAsync()
    {
        await respawner.ResetAsync(connection);
    }

    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
    }

    [Fact]
    public async Task IdentityService_UserRegistration_OkResult()
    {
        //Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "api/account/signup")
        {
            Content = JsonContent.Create(inputValue: new UserAuthenticationModel { Email = "myEmail@gmail.com", UserName = "Anton", Password = "123" })
        };

        //Act
        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        //Assert
        var authenticationResponse = await response.Content.ReadFromJsonAsync<UserAuthenticationResponse>();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        authenticationResponse.Should().NotBeNull();
    }

    [Fact]
    public async Task IdentityService_UserRegistration_FailedWhen_EmailAlreadyExists()
    {
        //Arrange
        var firstRequest = new HttpRequestMessage(HttpMethod.Post, "api/account/signup")
        {
            Content = JsonContent.Create(inputValue: new UserAuthenticationModel { Email = "myEmail@gmail.com", UserName = "Anton", Password = "123" })
        };

        var secondRequest = new HttpRequestMessage(HttpMethod.Post, "api/account/signup")
        {
            Content = JsonContent.Create(inputValue: new UserAuthenticationModel { Email = "myEmail@gmail.com", UserName = "Anton", Password = "123456" })
        };

        //Act
        var firstResponse = await httpClient.SendAsync(firstRequest);
        firstResponse.EnsureSuccessStatusCode();

        var secondResponse = await httpClient.SendAsync(secondRequest);

        //Assert
        secondResponse.StatusCode.Should().NotBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task IdentityService_UserLogin_OkResult()
    {
        //Arrange
        var registrationRequest = new HttpRequestMessage(HttpMethod.Post, "api/account/signup")
        {
            Content = JsonContent.Create(inputValue: new UserAuthenticationModel { Email = "myEmail@gmail.com", UserName = "Anton", Password = "123" })
        };

        var loginRequest = new HttpRequestMessage(HttpMethod.Post, "api/account/signin")
        {
            Content = JsonContent.Create(inputValue: new UserAuthenticationModel { Email = "myEmail@gmail.com", UserName = "Anton", Password = "123" })
        };

        //Act
        var registrationResponse = await httpClient.SendAsync(registrationRequest);
        registrationResponse.EnsureSuccessStatusCode();

        var loginResponse = await httpClient.SendAsync(loginRequest);
        loginResponse.EnsureSuccessStatusCode();

        //Assert
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var authenticationResponse = await loginResponse.Content.ReadFromJsonAsync<UserAuthenticationResponse>();
        authenticationResponse.Should().NotBeNull();
    }

    [Fact]
    public async Task IdentityService_UserLogin_FailedWhen_InvalidLoginCredentials()
    {
        //Arrange
        var loginRequest = new HttpRequestMessage(HttpMethod.Post, "api/account/signin")
        {
            Content = JsonContent.Create(inputValue: new UserAuthenticationModel { Email = "myEmail@gmail.com", UserName = "Anton", Password = "123" })
        };

        //Act
        var loginResponse = await httpClient.SendAsync(loginRequest);

        //Assert
        var authenticationResponse = await loginResponse.Content.ReadFromJsonAsync<UserAuthenticationResponse>();
        loginResponse.StatusCode.Should().NotBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task IdentityService_RefreshToken_OkResult()
    {
        //Arrange
        var registrationRequest = new HttpRequestMessage(HttpMethod.Post, "api/account/signup")
        {
            Content = JsonContent.Create(inputValue: new UserAuthenticationModel { Email = "myEmail@gmail.com", UserName = "Anton", Password = "123" })
        };

        //Act
        var registrationResponse = await httpClient.SendAsync(registrationRequest);
        registrationResponse.EnsureSuccessStatusCode();
        var authenticationResponse = await registrationResponse.Content.ReadFromJsonAsync<UserAuthenticationResponse>();

        httpClient.DefaultRequestHeaders.Add("Authorization", authenticationResponse!.Token.AccessToken);

        var refreshTokenRequest = new HttpRequestMessage(HttpMethod.Post, "api/account/refreshtoken")
        {
            Content = JsonContent.Create(inputValue: new RefreshTokenRequest { RefreshToken = authenticationResponse!.Token.RefreshToken})
        };

        var refreshTokenResponse = await httpClient.SendAsync(refreshTokenRequest);
        registrationResponse.EnsureSuccessStatusCode();

        //Assert
        registrationResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var latestAuthenticationResponse = await refreshTokenResponse.Content.ReadFromJsonAsync<UserAuthenticationResponse>();
        latestAuthenticationResponse.Should().NotBeNull();
    }

    [Fact]
    public async Task IdentityService_RefreshToken_FailedWhen_InvalidRefreshToken()
    {
        //Arrange
        var registrationRequest = new HttpRequestMessage(HttpMethod.Post, "api/account/signup")
        {
            Content = JsonContent.Create(inputValue: new UserAuthenticationModel { Email = "myEmail@gmail.com", UserName = "Anton", Password = "123" })
        };

        //Act
        var registrationResponse = await httpClient.SendAsync(registrationRequest);
        registrationResponse.EnsureSuccessStatusCode();
        var authenticationResponse = await registrationResponse.Content.ReadFromJsonAsync<UserAuthenticationResponse>();

        var serverTime = (FakeTimeProvider)_factory.Services.GetRequiredService<TimeProvider>();
        serverTime.AdvanceTime(TimeSpan.FromDays(31));

        httpClient.DefaultRequestHeaders.Add("Authorization", authenticationResponse!.Token.AccessToken);

        var refreshTokenRequest = new HttpRequestMessage(HttpMethod.Post, "api/account/refreshtoken")
        {
            Content = JsonContent.Create(inputValue: new RefreshTokenRequest { RefreshToken = authenticationResponse!.Token.RefreshToken })
        };

        var refreshTokenResponse = await httpClient.SendAsync(refreshTokenRequest);

        //Assert
        refreshTokenResponse.StatusCode.Should().NotBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task IdentityService_RefreshToken_ExpiredTokensAreDeleted()
    {
        //Arrange
        var jobClient = _factory.Services.GetRequiredService<IBackgroundJobClient>();
        var registrationRequest = new HttpRequestMessage(HttpMethod.Post, "api/account/signup")
        {
            Content = JsonContent.Create(inputValue: new UserAuthenticationModel { Email = "myEmail@gmail.com", UserName = "Anton", Password = "123" })
        };

        //Act
        var registrationResponse = await httpClient.SendAsync(registrationRequest);
        registrationResponse.EnsureSuccessStatusCode();

        var serverTime = (FakeTimeProvider)_factory.Services.GetRequiredService<TimeProvider>();
        serverTime.AdvanceTime(TimeSpan.FromDays(32));

        var job = _factory.Services.GetRequiredService<IMockRefreshTokenCleanUpService>();
        BackgroundJob.Enqueue(() => job.ClearExpiredTokensAsync());

        await Task.Delay(TimeSpan.FromSeconds(5));

        var loginRequest = new HttpRequestMessage(HttpMethod.Post, "api/account/signin")
        {
            Content = JsonContent.Create(inputValue: new UserAuthenticationModel { Email = "myEmail@gmail.com", UserName = "Anton", Password = "123" })
        };

        var loginResponse = await httpClient.SendAsync(loginRequest);

        //Assert
        loginResponse.StatusCode.Should().NotBe(HttpStatusCode.OK);
    }
}
