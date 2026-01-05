using QuickTalk.Client.Authentication.Dto;

namespace QuickTalk.Client.Authentication.Interfaces;

public interface IAuthenticationService
{
    Task<AuthenticationResponse> LoginAsync(LoginRequest model);
    Task<AuthenticationResponse> RegisterAsync(RegistrationRequest model);
    Task LogOutAsync();
    Task<string> RefreshTokenAsync(string refreshToken);
}
