using QuickTalk.Identity.Domain.Entities;

namespace QuickTalk.Identity.WebApi.Dto;

public class AuthenticationResponse
{
    public Token Token { get; set; } = null!;
}
