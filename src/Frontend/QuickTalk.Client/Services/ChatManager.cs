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

    public async Task SendMessageAsync(MessageDto content)
    {
        try
        {
            var client = factory.CreateClient("WebApi");
            var response = await client.PutAsync("api/messages", JsonContent.Create(content));
            response.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            throw;
        }
    }

    public async Task<List<MessageDto>?> GetMessagesAsync()
    {
        try
        {
            var client = factory.CreateClient("WebApi");
            var response = await client.GetAsync($"api/messages");
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
