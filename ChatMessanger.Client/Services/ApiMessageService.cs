using ChatMessanger.Client.Models;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;

namespace ChatMessanger.Client.Services
{
    public class ApiMessageService
    {
        private readonly HttpClient httpClient;
        public ApiMessageService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task SaveMessageAsync(MessageDTO messageDTO)
        {
            try
            {
                JsonContent msgContent = JsonContent.Create(messageDTO);
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
                    var messages = await response.Content.ReadFromJsonAsync<List<MessageDTO>>();
                    return messages;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return new List<MessageDTO>();
        }
    }
}
