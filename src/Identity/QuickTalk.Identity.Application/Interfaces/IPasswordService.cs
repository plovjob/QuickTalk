namespace QuickTalk.Identity.Application.Interfaces;

public interface IPasswordService
{
    (string Password, string Salt) HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword, string salt);
}
