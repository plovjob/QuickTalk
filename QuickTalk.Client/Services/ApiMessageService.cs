using QuickTalk.Client.Models;
using System.Net.Http.Json;

namespace ChatMessanger.Client.Services;

public class ApiMessageService(HttpClient httpClient)
{
    public async Task SaveMessageAsync(MessageDTO messageDTO)
    {
        try
        {
            var msgContent = JsonContent.Create(messageDTO);
            await httpClient.PostAsync("https://localhost:7005/api/msgs/save", msgContent);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public async Task<List<MessageDTO>> ShowMessageHistory()
    {
        try
        {
            var response = await httpClient.GetAsync("https://localhost:7005/api/msgs/showall");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<MessageDTO>>();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return new List<MessageDTO>();
    }
}
