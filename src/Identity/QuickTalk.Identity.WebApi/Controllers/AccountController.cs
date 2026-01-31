using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickTalk.Identity.Application.Interfaces;
using QuickTalk.Identity.Domain.Entities;
using QuickTalk.Identity.WebApi.Dto;
using QuickTalk.Shared.Messaging;

namespace QuickTalk.Identity.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(
    IAuthenticationService authenticationService,
    ILogger<AccountController> logger,
    IPublishEndpoint publishEndpoint) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] RegistrationRequest request)
    {
        var signUpResult = await authenticationService.SignUpAsync(request.Email, request.UserName, request.Password);

        if (signUpResult.IsFailure && signUpResult.Error != null)
        {
            return ToProblemResponse(signUpResult.Error);
        }

        await publishEndpoint.Publish<IUserHelloMessage>(new {Message = $"Hello, {request.Email}"});

        return Ok(new AuthenticationResponse
        {
            Token = signUpResult!.Data!.Token,
            UserId = signUpResult.Data.UserId,
            UserName = signUpResult.Data.UserName
        });
    }

    [AllowAnonymous]
    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] LoginResponse request)
    {
        var signInResult = await authenticationService.SignInAsync(request.Email, request.Password);

        if (signInResult.IsFailure && signInResult.Error != null)
        {
            return ToProblemResponse(signInResult.Error);
        }

        await publishEndpoint.Publish<IUserHelloMessage>(new { Message = $"Hello, {request.Email}" });

        return Ok(new AuthenticationResponse { Token = signInResult!.Data! });
    }

    [AllowAnonymous]
    [HttpPost("refreshtoken")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
    {
        var result = await authenticationService.RefreshTokenAsync(request.RefreshToken);

        if (!result.IsSuccess)
        {
            return ToProblemResponse(UserErrors.InvalidRefreshToken);
        }

        return Ok(new AuthenticationResponse { Token = result!.Data! });
    }

    private IActionResult ToProblemResponse(Error error)
    {
        logger.LogError($"One or more errors while processing the request: {error.Description}");

        return Problem(
            title: error.Description,
            statusCode: error.StatusCode);
    }
}
