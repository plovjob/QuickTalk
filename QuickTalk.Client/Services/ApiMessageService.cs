using QuickTalk.Client.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Json;
using System.Text;

namespace QuickTalk.Client.Services;

internal sealed class ApiMessageService(HttpClient httpClient)
{
    JsonSerializerSettings jsonSerializerSettings = new()
    {
        ContractResolver = new DefaultContractResolver(),
    };

    internal async Task SendMessageAsync(Content content)
    {
        try
        {
            var messageJson = JsonConvert.SerializeObject(content, jsonSerializerSettings);
            var messageJsonContent = new StringContent(messageJson, Encoding.UTF8, "application/json");
            await httpClient.PostAsync($"{httpClient.BaseAddress}api/message/send", messageJsonContent);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    internal async Task<List<MessageDto>?> GetMessagesAsync()
    {
        try
        {
            var response = await httpClient.GetAsync($"{httpClient.BaseAddress}api/message/get");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<MessageDto>>();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return new List<MessageDto>();
    }
}
