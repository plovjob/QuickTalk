namespace QuickTalk.Client.Services;

public class TestService(IHttpClientFactory factory)
{
    public async Task SendTestRequestAsync()
    {
        var client = factory.CreateClient("WebApi");
        var response = await client.GetAsync("api/messages/testmethod");
    }
}
