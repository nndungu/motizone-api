using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MotiZone.Models;

namespace MotiZone.Services
{
    public class MpesaService
    {
        private readonly HttpClient _httpClient;
        private readonly MpesaLogService _mpesaLogService;
        private readonly IConfiguration _configuration;

        public MpesaService(HttpClient httpClient, MpesaLogService mpesaLogService, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _mpesaLogService = mpesaLogService;
            _configuration = configuration;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var consumerKey = _configuration["Mpesa:ConsumerKey"];
            var consumerSecret = _configuration["Mpesa:ConsumerSecret"];
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{consumerKey}:{consumerSecret}"));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            var response = await _httpClient.GetAsync("https://sandbox.safaricom.co.ke/oauth/v1/generate?grant_type=client_credentials");

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);
            return json.RootElement.GetProperty("access_token").GetString();
        }

        public async Task<bool> InitiateStkPush(string phoneNumber, decimal amount)
        {
            var token = await GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var shortcode = _configuration["Mpesa:ShortCode"];
            var passkey = _configuration["Mpesa:PassKey"];
            var password = Convert.ToBase64String(Encoding.UTF8.GetBytes(shortcode + passkey + timestamp));

            var payload = new
            {
                BusinessShortCode = shortcode,
                Password = password,
                Timestamp = timestamp,
                TransactionType = "CustomerPayBillOnline",
                Amount = amount,
                PartyA = phoneNumber,
                PartyB = shortcode,
                PhoneNumber = phoneNumber,
                CallBackURL = _configuration["Mpesa:CallbackUrl"],
                AccountReference = "MotiZone",
                TransactionDesc = "Vehicle/Part Purchase"
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://sandbox.safaricom.co.ke/mpesa/stkpush/v1/processrequest", content);
            
            var responseBody = await response.Content.ReadAsStringAsync();
            return responseBody.Contains("success");
        }
        
        /*[HttpPost]
        public async Task<IActionResult> MpesaConfirmation([FromBody] MpesaCallbackResponse resp)
        {
            await _mpesaLogService.LogCallbackAsync(resp);
            var payment = new Payment {
                OrderRef = resp.Body.StkCallback.CheckoutRequestID,
                Amount = resp.Body.StkCallback.Amount,
                PaymentMethod = "Mpesa",
                PaidAt = DateTime.UtcNow,
                Status = resp.Body.StkCallback.ResultCode == 0 ? "Paid" : "Failed"
            };
            await _paymentService.CreateAsync(payment);
            return Ok(new { ResultCode = 0, ResultDesc = "Accepted" });
        }*/
    }
}
