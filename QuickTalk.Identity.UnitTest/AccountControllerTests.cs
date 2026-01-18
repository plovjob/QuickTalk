using FluentAssertions;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using QuickTalk.Identity.Application.Interfaces;
using QuickTalk.Identity.Domain.Entities;
using QuickTalk.Identity.WebApi.Controllers;
using QuickTalk.Identity.WebApi.Dto;
using QuickTalk.Shared.Messaging;

namespace QuickTalk.Identity.UnitTests;

public class AccountControllerTests
{
    private readonly Mock<IAuthenticationService> mockAuthenticationService;
    private readonly Mock<ILogger<AccountController>> mockLogger;
    private readonly Mock<IPublishEndpoint> mockPublishEndpoint;
    private readonly AccountController accountController;
    private readonly Mock<TimeProvider> mockTimeProvider;

    public AccountControllerTests()
    {
        mockAuthenticationService = new();
        mockLogger = new();
        mockPublishEndpoint = new();
        mockTimeProvider = new();

        accountController = new(mockAuthenticationService.Object, mockLogger.Object, mockPublishEndpoint.Object);

        var fixedTime = new DateTimeOffset(2024, 1, 15, 10, 0, 0, TimeSpan.Zero);

        mockTimeProvider.Setup(s => s.GetUtcNow()).Returns(fixedTime);
    }

    [Fact]
    public async Task SignIn_When_UserFound_AndCorrectPassword_Returns_OkObjectResult_WithTokensPairValue()
    {
        //Arrange
        var loginRequest = new LoginRequest() { Email = "@Example", Password = "password" };
        var tokenResponse = new Token(AccessToken: "AccessToken", RefreshToken: "RefreshToken", ExpirationTime: mockTimeProvider.Object.GetUtcNow().DateTime);
        var authenticationResponse = new AuthenticationResponse() { Token = tokenResponse };

        mockAuthenticationService.Setup(s => s.SignInAsync(loginRequest.Email, loginRequest.Password).Result).Returns(Result<Token?>.Success(tokenResponse));

        //Act
        var signInResult = await accountController.SignIn(loginRequest);

        //Assert
        var actionResult = signInResult.Should().BeAssignableTo<OkObjectResult>().Subject;
        actionResult.StatusCode.Should().Be(200);
        actionResult.Value.Should().NotBeNull();
        actionResult.Value.Should().BeOfType<AuthenticationResponse>().Which.Should().BeEquivalentTo(authenticationResponse);

        mockPublishEndpoint.Verify(x => x.Publish<IUserLoggedIn>(It.IsAny<object>(), default), Times.Once);
    }

    [Fact]
    public async Task SignIn_When_UserNotFound_Returns_ProblemDetails_With_NotFoundStatusCode_EventIsNotCalled()
    {
        //Arrange
        var loginRequest = new LoginRequest() { Email = "@Example", Password = "password" };

        mockAuthenticationService.Setup(s => s.SignInAsync(loginRequest.Email, loginRequest.Password).Result).Returns(Result<Token?>.Failure(UserErrors.UserNotFound(loginRequest.Email)));

        //Act
        var signInResult = await accountController.SignIn(loginRequest);

        //Assert
        var problemResult = signInResult.Should().BeAssignableTo<ObjectResult>().Subject.Value.Should().BeAssignableTo<ProblemDetails>().Subject;
        problemResult.Status.Should().Be(404);

        mockPublishEndpoint.Verify(x => x.Publish<IUserLoggedIn>(It.IsAny<object>(), default), Times.Never);
    }

    [Fact]
    public async Task SignIn_When_InvalidPassword_Returns_ProblemDetails_With_UnAuthorizedStatusCode_EventIsNotCalled()
    {
        //Arrange
        var loginRequest = new LoginRequest() { Email = "@Example", Password = "password" };

        mockAuthenticationService.Setup(s => s.SignInAsync(loginRequest.Email, loginRequest.Password).Result).Returns(Result<Token?>.Failure(UserErrors.Unauthorized));

        //Act
        var signInResult = await accountController.SignIn(loginRequest);

        //Assert
        var problemResult = signInResult.Should().BeAssignableTo<ObjectResult>().Subject.Value.Should().BeAssignableTo<ProblemDetails>().Subject;
        problemResult.Status.Should().Be(401);

        mockPublishEndpoint.Verify(x => x.Publish<IUserLoggedIn>(It.IsAny<object>(), default), Times.Never);
    }
}
