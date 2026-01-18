using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using QuickTalk.Identity.Application.Interfaces;
using QuickTalk.Identity.Domain.Entities;
using QuickTalk.Identity.UnitTest.Dto;
using QuickTalk.Identity.WebApi.Controllers;
using QuickTalk.Shared.Messaging;

namespace QuickTalk.Identity.UnitTest;

public class AccountControllerTests
{
    private readonly Mock<IAuthenticationService> mockAuthenticationService;
    private readonly Mock<ILogger<AccountController>> mockLogger;
    private readonly Mock<IPublishEndpoint> mockPublishEndpoint;
    private readonly AccountController accountController;

    public AccountControllerTests()
    {
        mockAuthenticationService = new();
        mockLogger = new();
        mockPublishEndpoint = new();

        accountController = new(mockAuthenticationService.Object, mockLogger.Object, mockPublishEndpoint.Object);
    }

    [Fact]
    public void SignIn_When_UserFound_AndCorrectPassword_Returns()
    {
        //Arrange
        var loginRequest = new LoginRequest() { Email = "@Example", Password = "password" };

        mockAuthenticationService.Setup(s => s.SignInAsync(loginRequest.Email, loginRequest.Password).Result).Returns(Result<Token?>.Success(It.IsAny<Token>()));

        mockPublishEndpoint.Verify(x => x.Publish<IUserLoggedIn>(It.IsAny<IUserLoggedIn>()))
    }
}
