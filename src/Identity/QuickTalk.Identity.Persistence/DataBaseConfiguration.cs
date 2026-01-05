namespace QuickTalk.Identity.Persistence;

public record DataBaseConfiguration
{
    public string? Host { get; set; }
    public string? Port { get; set; }
    public string? Database { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }

    public override string ToString()
    {
        return $"Host={Host}; Port={Port}; Database={Database}; Username={UserName}; Password={Password};";
    }
}
