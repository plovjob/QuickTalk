using System.Security.Cryptography;
using System.Text;
using QuickTalk.Identity.Application.Interfaces;

namespace QuickTalk.Identity.Application.Services;

public class PasswordService : IPasswordService
{
    private const int hashSize = 32;
    private const int saltSize = 16;
    private const int iterations = 10000;

    public (string Password, string Salt) HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(saltSize);
        var passwordSalt = Convert.ToBase64String(salt);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            hashSize);

        return (Password: Convert.ToBase64String(hash), Salt: passwordSalt);
    }

    public bool VerifyPassword(string password, string hashedPassword, string salt)
    {
        var byteSalt = Convert.FromBase64String(salt);
        var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, byteSalt, iterations, HashAlgorithmName.SHA256, hashSize);
        return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromBase64String(hashedPassword));
    }
}
