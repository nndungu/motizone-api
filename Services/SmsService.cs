using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;

namespace MotiZone.Services;

public class SmsService
{
    private readonly IConfiguration _config;
    public SmsService(IConfiguration config) => _config = config;

    public async Task SendSmsAsync(string phoneNumber, string message)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("apiKey", _config["AfricaTalking:ApiKey"]);
        var payload = new Dictionary<string, string>
        {
            ["username"] = _config["AfricaTalking:Username"],
            ["to"] = phoneNumber,
            ["message"] = message
        };
        await client.PostAsync("https://api.africastalking.com/version1/messaging", new FormUrlEncodedContent(payload));
    }
}