using Microsoft.AspNetCore.SignalR.Client;
using QuickTalk.Client.Authentication.Interfaces;

namespace QuickTalk.Client.Services;

public class HubService(
    ILogger<HubService> logger,
    ApiBaseAddressConfiguration configuration,
    ISessionTokenStorage sessionTokenStorage) : IAsyncDisposable
{
    public HubConnection Connection { get; private set; } = null!;
    public bool IsConnected => Connection?.State == HubConnectionState.Connected;

    public event Action<string, string>? UserMessageReceived;
    public event Action<string>? HelloMessageReceived;

    public async Task InitializeAsync()
    {
        try
        {
            var token = await sessionTokenStorage.GetAccessTokenAsync();

            Connection = new HubConnectionBuilder()
             .WithUrl($"{configuration.MessagesWebApiUrl}/chathub", options =>
             {
                 options.AccessTokenProvider = () => Task.FromResult<string?>(token);
             })
             .WithAutomaticReconnect(new[]
             {
                 TimeSpan.Zero,
                 TimeSpan.FromSeconds(2),
                 TimeSpan.FromSeconds(5),
                 TimeSpan.FromSeconds(10)
             })
             .AddJsonProtocol(options =>
             {
                 options.PayloadSerializerOptions.PropertyNamingPolicy =
                     System.Text.Json.JsonNamingPolicy.CamelCase;
             })
             .Build();

            Connection.On<string, string>("ReceiveMessage", (userName, message) =>
            {
                UserMessageReceived?.Invoke(userName, message);
            });

            Connection.On<string>("ReceiveHelloMessage", (helloMessage) =>
            {
                logger.LogInformation($"Получено приветственное сообщение: {helloMessage}");
                HelloMessageReceived?.Invoke(helloMessage);
            });

            await Connection.StartAsync();

            logger.LogInformation("SignalR подключен. ConnectionId: {ConnectionId}",
                Connection.ConnectionId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка подключения к SignalR");
            throw;
        }
    }

    public async Task SendMessageAsync(string userName, string message)
    {
        await Connection.SendAsync("SendMessageAsync", userName, message);
    }

    public async Task SendHelloMessageAsync()
    {
        if (!IsConnected)
        {
            throw new InvalidOperationException("Нет подключения");
        }

        await Connection.SendAsync("SendHelloMessageAsync");
    }

    public async Task DisconnectAsync()
    {
        if (IsConnected)
        {
            await Connection.StopAsync();
            await Connection.DisposeAsync();
        }
    }

    public async ValueTask DisposeAsync() => await DisconnectAsync();
}
