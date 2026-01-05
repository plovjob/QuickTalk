using System.Text.Json.Serialization;

namespace QuickTalk.Identity.Domain.Entities;

public class User(Guid id, DateTime createdAt, string userName, string email, string passwordHash, string passwordSalt)
{
    public Guid Id { get; private set; } = id;
    public string Email { get; private set; } = email;
    public string UserName { get; set; } = userName;
    public DateTime CreatedAt { get; private set; } = createdAt;

    [JsonIgnore]
    public string PasswordHash { get; set; } = passwordHash;

    [JsonIgnore]
    public string PasswordSalt { get; set; } = passwordSalt;

    public static User Create(string email, DateTime createdAt, string userName, string passwordHash, string passwordSalt)
    {
        return new User(
            Guid.NewGuid(),
            createdAt,
            userName,
            email,
            passwordHash,
            passwordSalt);
    }
}
