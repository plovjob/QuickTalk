//using System.Security.Claims;
//using FluentAssertions;
//using Moq;
//using QuickTalk.Identity.Application.Interfaces;
//using QuickTalk.Identity.Application.Models;
//using QuickTalk.Identity.Application.Services;
//using QuickTalk.Identity.Domain.Entities;

//namespace QuickTalk.Identity.UnitTests;

//public class AuthenticationServiceTests
//{
//    private readonly Mock<IUserService> mockUserService;
//    private readonly Mock<ITokenService> mockTokenService;
//    private readonly Mock<JwtConfig> mockJwtConfig;
//    private readonly Mock<TimeProvider> mockTimeProvider;
//    private readonly Mock<IPasswordService> mockPasswordService;
//    private readonly Mock<IRefreshTokenService> mockRefreshTokenService;
//    private readonly AuthenticationService authenticationService;

//    public AuthenticationServiceTests()
//    {
//        mockUserService = new();
//        mockTokenService = new();
//        mockJwtConfig = new();
//        mockPasswordService = new();
//        mockRefreshTokenService = new();
//        mockTimeProvider = new();

//        authenticationService = new(
//            mockUserService.Object,
//            mockTokenService.Object,
//            mockJwtConfig.Object,
//            mockTimeProvider.Object,
//            mockPasswordService.Object,
//            mockRefreshTokenService.Object);

//        var fixedTime = new DateTimeOffset(2024, 1, 15, 10, 0, 0, TimeSpan.Zero);

//        mockTimeProvider.Setup(s => s.GetUtcNow()).Returns(fixedTime);
//    }

//    [Fact]
//    public async Task SignIn_When_UserFound_AndCorrectPassword_Returns_TokenPair()
//    {
//        //Arrange
//        var login = "@Example";
//        var userName = "Anton";
//        var password = "123";
//        var hashedPassword = "Hashed123";
//        var saltedPassword = "Salted123";
//        var token = "Token123";
//        var refreshToken = "Refresh123";

//        var user = User.Create(login, mockTimeProvider.Object.GetUtcNow().DateTime, userName, hashedPassword, saltedPassword);

//        mockUserService.Setup(s => s.GetAuthenticatedUserAsync(user.Email).Result).Returns(Result<User?>.Success(user));

//        mockPasswordService.Setup(s => s.VerifyPassword(password, hashedPassword, saltedPassword)).Returns(true);

//        mockTokenService.Setup(s => s.CreateToken(It.IsAny<Claim[]>())).Returns(token);

//        mockTokenService.Setup(s => s.BuildRefreshToken()).Returns(refreshToken);

//        mockRefreshTokenService.Setup(s => s.UpdateRefreshTokenAsync(It.IsAny<RefreshToken>()).Result).Returns(Result.Success());

//        //Act
//        var result = await authenticationService.SignInAsync(user.Email, password);

//        //Assert
//        var resultDataTokensPair = result.Data.Should().BeOfType<Token>().Subject;
//        resultDataTokensPair.AccessToken.Should().Be(token);
//        resultDataTokensPair.RefreshToken.Should().Be(refreshToken);
//    }

//    [Fact]
//    public async Task SignIn_When_UserNotFound_Returns_NotFoundStatusCode()
//    {
//        //Arrange
//        var login = "@Example";
//        var userName = "Anton";

//        mockUserService.Setup(s => s.GetAuthenticatedUserAsync(login).Result).Returns(Result<User?>.Success(null));

//        //Act
//        var result = await authenticationService.SignInAsync(login, userName);

//        //Assert
//        var resultData = result.Should().BeOfType<Result<Token?>>().Subject;
//        result.IsFailure.Should().Be(true);
//        result.Error?.Should().NotBe(null);
//        result.Error?.StatusCode.Should().Be(404);
//    }

//    [Fact]
//    public async Task SignIn_When_InvalidPassword_Returns_UnAuthorizedStatusCode()
//    {
//        //Arrange
//        var login = "@Example";
//        var userName = "Anton";
//        var password = "123";
//        var hashedPassword = "Hashed123";
//        var saltedPassword = "Salted123";

//        var user = User.Create(login, mockTimeProvider.Object.GetUtcNow().DateTime, userName, hashedPassword, saltedPassword);

//        mockUserService.Setup(s => s.GetAuthenticatedUserAsync(user.Email).Result).Returns(Result<User?>.Success(user));

//        mockPasswordService.Setup(s => s.VerifyPassword(password, hashedPassword, saltedPassword)).Returns(false);

//        //Act
//        var result = await authenticationService.SignInAsync(login, userName);
//        //Assert
//        var resultData = result.Should().BeOfType<Result<Token?>>().Subject;
//        resultData.IsFailure.Should().BeTrue();
//        result.Data.Should().BeNull();
//        result.Error?.Should().NotBe(null);
//        result.Error?.StatusCode.Should().Be(401);
//    }
//}
