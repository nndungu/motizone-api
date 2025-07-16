/*
using Microsoft.AspNetCore.Mvc;
using MotiZone.Data;
using MotiZone.Models;
using MotiZone.Services;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;
using Stripe.Forwarding;
using StripeRequest = MotiZone.Models.StripeRequest;

namespace MotiZone.Controllers;

public class PaymentIntegrationController
{
    private readonly MpesaService _mpesaService;
    private readonly StripeService _stripeService;
    private readonly IConfiguration _configuration;

    public PaymentIntegrationController(MpesaService mpesaService, StripeService stripeService,
        IConfiguration configuration)
    {
        _mpesaService = mpesaService;
        _stripeService = stripeService;
        _configuration = configuration;
    }
    [HttpPost]
    public async Task<IActionResult> InitiateMpesaStkPush([FromBody] MpesaRequest request)
    {
        var checkoutRequestId = await _mpesaService.InitiateStkPush(request.Amount, request.PhoneNumber, request.OrderRef);
        return Ok(new { checkoutRequestId });
    }

    [HttpPost]
    public async Task<IActionResult> CreateStripeCheckout([FromBody] StripeRequest request)
    {
        var url = await _stripeService.CreateCheckoutSession(request.ProductName, request.AmountCents,
            request.SuccessUrl, request.CancelUrl);
        return Ok(new { checkoutUrl = url });
    }

    [HttpPost]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var signature = Request.Headers["Stripe-Signature"];
        var webhookSecret = _configuration["Stripe:WebhookSecret"];

        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(json, signature, webhookSecret);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }

        if (stripeEvent.Type == Events.CheckoutSessionCompleted)
        {
            var session = stripeEvent.Data.Object as Session;
            var userIdStr = session?.ClientReferenceId;

            if (int.TryParse(userIdStr, out int userId))
            {
                var amount = session.AmountTotal / 100.0m;
                var paymentService = HttpContext.RequestServices.GetRequiredService<PaymentService>();
                await paymentService.CreatePaymentAsync(userId, amount, "Stripe");
            }
        }

        return Ok();
    }
    
    [HttpPost("mpesa/callback")]
    public async Task<IActionResult> MpesaCallback([FromBody] dynamic data)
    {
        var context = HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();

        try
        {
            var callback = data.Body.stkCallback;
            var mpesaTxn = new MpesaTransaction
            {
                TransactionId = callback.CheckoutRequestId,
                PhoneNumber = callback.CallbackMetadata?.Item?[1]?.Value,
                Amount = Convert.ToDecimal(callback.CallbackMetadata?.Item?[0]?.Value),
                Status = callback.ResultDesc,
                Timestamp = DateTime.Now
            };
            context.MpesaTransactions.Add(mpesaTxn);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Mpesa callback error: " + ex.Message);
            // return BadRequest(new { message = ex.Message });
        }
        return Ok(new { ResultCode = 0, ResultDesc = "Success" });
    }
}
*/
