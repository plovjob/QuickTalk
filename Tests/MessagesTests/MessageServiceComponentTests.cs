using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using QuickTalk.Messages.ComponentTests.Infrastructure;
using QuickTalk.Messages.ComponentTests.Infrastructure.MockTime;
using QuickTalk.Messages.ComponentTests.Models;

namespace QuickTalk.Messages.ComponentTests;

public class MessageServiceComponentTests(ApiMessageWebApplicationFactory factory) : IClassFixture<ApiMessageWebApplicationFactory>, IAsyncLifetime
{
    private HttpClient _httpClient = factory.CreateClient();

    private readonly JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task MessageService_GetMessagesFromDb_ReturnsEmptyCollection_OkResult()
    {
        var response = await SendRequestAsync(HttpMethod.Get);
        response.EnsureSuccessStatusCode();
        var responseJson = await response.Content.ReadAsStringAsync();
        var responseMessageCollection = JsonSerializer.Deserialize<IEnumerable<MessageServiceOutputModel>>(responseJson, options);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseMessageCollection.Should().BeEmpty();
    }

    [Fact]
    public async Task MessageService_MessageSavedWith_ValidData()
    {
        var timeProvider = factory.Services.GetRequiredService<TimeProvider>();
        var serverTime = timeProvider as FakeTimeProvider ?? throw new InvalidCastException("error converting timeprovider.system object to faketimeprovider");

        var serviceInput = new MessageServiceInputModel(Guid.NewGuid(), "Anton", "Hello world!");

        var sendMessageResponse = await SendRequestAsync(HttpMethod.Put, serviceInput);
        sendMessageResponse.EnsureSuccessStatusCode();

        var getMessageResponse = await SendRequestAsync(HttpMethod.Get);
        getMessageResponse.EnsureSuccessStatusCode();

        var messageResponseJson = await getMessageResponse.Content.ReadAsStringAsync();
        var messages = JsonSerializer.Deserialize<IReadOnlyCollection<MessageServiceOutputModel>>(messageResponseJson, options);
        var targetMessage = messages!.FirstOrDefault(e => e.Id == serviceInput.Id);

        messages.Should().NotBeNullOrEmpty();
        targetMessage!.Id.Should().Be(serviceInput.Id);
        targetMessage!.UserName.Should().Be(serviceInput.UserName);
        targetMessage!.Text.Should().Be(serviceInput.Text);
        targetMessage!.SentAt.Should().Be(serverTime.messagesSentTimeHistory[0].UtcDateTime);
    }

    [Fact]
    public async Task MessageService_CreateTwoMessages_StoredInTheOrderTheyWereSent_ValidData()
    {
        var timeProvider = factory.Services.GetRequiredService<TimeProvider>();
        var serverTime = timeProvider as FakeTimeProvider ?? throw new InvalidCastException("error converting timeprovider.system object to faketimeprovider");

        var serviceInputFirstMessage = new MessageServiceInputModel(Guid.NewGuid(), "first user", "first user text");
        var serviceInputSecondMessage = new MessageServiceInputModel(Guid.NewGuid(), "second user", "second user text");

        var firstResponse = await SendRequestAsync(HttpMethod.Put, serviceInputFirstMessage);
        firstResponse.EnsureSuccessStatusCode();
        var firstEntitySentTime = serverTime.messagesSentTimeHistory[0];

        var secondResponse = await SendRequestAsync(HttpMethod.Put, serviceInputSecondMessage);
        secondResponse.EnsureSuccessStatusCode();
        var secondEntitySentTime = serverTime.messagesSentTimeHistory[1];

        var getMessageResponse = await SendRequestAsync(HttpMethod.Get);
        getMessageResponse.EnsureSuccessStatusCode();
        var messageResponseJson = await getMessageResponse.Content.ReadAsStringAsync();
        var messages = JsonSerializer.Deserialize<IReadOnlyCollection<MessageServiceOutputModel>>(messageResponseJson, options);

        messages.Should().NotBeNullOrEmpty();

        var firstEntity = messages.FirstOrDefault(e => e.Id == serviceInputFirstMessage.Id);
        var secondEntity = messages.FirstOrDefault(e => e.Id == serviceInputSecondMessage.Id);

        firstEntity.Should().NotBeNull();
        firstEntity.Id.Should().Be(serviceInputFirstMessage.Id);
        firstEntity.Text.Should().Be(serviceInputFirstMessage.Text);
        firstEntity.UserName.Should().Be(serviceInputFirstMessage.UserName);
        firstEntity.SentAt.Should().Be(firstEntitySentTime.UtcDateTime);

        secondEntity.Should().NotBeNull();
        secondEntity.Id.Should().Be(serviceInputSecondMessage.Id);
        secondEntity.Text.Should().Be(serviceInputSecondMessage.Text);
        secondEntity.UserName.Should().Be(serviceInputSecondMessage.UserName);
        secondEntity.SentAt.Should().Be(secondEntitySentTime.UtcDateTime);
    }

    [Fact]
    public async Task MessageService_UpdatingMessageWithSameId_ValidData()
    {
        var timeProvider = factory.Services.GetRequiredService<TimeProvider>();
        var serverTime = timeProvider as FakeTimeProvider ?? throw new InvalidCastException("error converting timeprovider.system object to faketimeprovider");

        var serviceInputDuplicateMessage = new MessageServiceInputModel(Guid.NewGuid(), "first user", "first user text");
        var firstResponse = await SendRequestAsync(HttpMethod.Put, serviceInputDuplicateMessage);
        firstResponse.EnsureSuccessStatusCode();

        var secondResponse = await SendRequestAsync(HttpMethod.Put, serviceInputDuplicateMessage);
        secondResponse.EnsureSuccessStatusCode();

        var getMessageResponse = await SendRequestAsync(HttpMethod.Get);
        getMessageResponse.EnsureSuccessStatusCode();
        var messageResponseJson = await getMessageResponse.Content.ReadAsStringAsync();
        var messages = JsonSerializer.Deserialize<IReadOnlyCollection<MessageServiceOutputModel>>(messageResponseJson, options);

        messages.Should().NotBeNullOrEmpty();
        messages.Count.Should().Be(1);

        var message = messages!.FirstOrDefault(e => e.Id == serviceInputDuplicateMessage.Id);

        message.Should().NotBeNull();
        message.Id.Should().Be(serviceInputDuplicateMessage.Id);
        message.UserName.Should().Be(serviceInputDuplicateMessage.UserName);
        message.Text.Should().Be(serviceInputDuplicateMessage.Text);
        message.SentAt.Should().Be(serverTime.messagesSentTimeHistory[0].UtcDateTime);
        message.EditedAt.Should().Be(serverTime.messagesSentTimeHistory[1].UtcDateTime);
    }

    async Task IAsyncLifetime.InitializeAsync()
    {
        await factory.InitializeAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await factory.ResetDatabaseAsync();
    }

    private async Task<HttpResponseMessage> SendRequestAsync(HttpMethod httpMethod, MessageServiceInputModel? message = default, string address = "/api/messages")
    {
        try
        {
            var request = new HttpRequestMessage(httpMethod, address);

            if (message != null && httpMethod != HttpMethod.Get)
            {
                request.Content = JsonContent.Create(inputValue: message, options: options);
            }

            return await _httpClient.SendAsync(request);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
