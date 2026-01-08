using System.Collections.Concurrent;
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

    private readonly ConcurrentQueue<string> _messageQueue = new();

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

    private void SetupEventHandlers()
    {
        Connection.On<string, string>("ReceiveMessage", (userName, message) =>
        {
            logger.LogInformation($"Получено сообщение от пользователя: {message}");
            UserMessageReceived?.Invoke(userName, message);
        });

        Connection.On<string>("ReceiveHelloMessage", (helloMessage) =>
        {
            logger.LogInformation($"Получено приветственное сообщение: {helloMessage}");
            HelloMessageReceived?.Invoke(helloMessage);
        });

        Connection.Reconnected += connectionId =>
        {
            logger.LogInformation("Переподключение успешно. Новый ConnectionId: {ConnectionId}",
                connectionId);
            return Task.CompletedTask;
        };

        Connection.Closed += error =>
        {
            logger.LogError(error, "Соединение закрыто");
            return Task.CompletedTask;
        };
    }

    //методы для отправки сообщений на сервер
    public async Task SendMessageAsync(string userName, string message)
    {
        if (!IsConnected)
        {
            throw new InvalidOperationException("Нет подключения");
        }

        //я это отправляю на сервер чтобы у меня отработал обработчик на клиенте
        await Connection.SendAsync("SendMessageAsync", userName, message);
    }

    public async Task NotifyAsync()
    {
        if (!IsConnected)
        {
            throw new InvalidOperationException("Нет подключения");
        }

        await Connection.SendAsync("NotifyAsync");
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
