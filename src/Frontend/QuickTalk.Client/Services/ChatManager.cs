using System.Net.Http.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using QuickTalk.Client.Interfaces;
using QuickTalk.Client.Models;

namespace QuickTalk.Client.Services;

internal sealed class ChatManager(IHttpClientFactory factory, ILogger<ChatManager> logger) : IChatManager
{
    private readonly JsonSerializerSettings jsonSerializeSettings = new()
    {
        ContractResolver = new DefaultContractResolver(),
    };

    public async Task SendMessageAsync(MessageDto messageDto)
    {
        try
        {
            var client = factory.CreateClient("WebApi");
            var response = await client.PutAsync("api/messages", JsonContent.Create(messageDto));
            response.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            throw;
        }
    }

    public async Task<List<MessageDto>?> GetMessagesAsync(Guid senderId, Guid consumerId)
    {
        try
        {
            var client = factory.CreateClient("WebApi");

            var senderIdParam = senderId.ToString();
            var consumerIdParam = consumerId.ToString();
            //var url = $"{client.BaseAddress}/api/messages?sender={senderIdParam}&consumer={consumerIdParam}";

            var response = await client.GetAsync($"api/messages?sender={senderIdParam}&consumer={consumerIdParam}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<MessageDto>>();
            }
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            throw;
        }

        return new List<MessageDto>();
    }
}
