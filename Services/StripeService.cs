using MotiZone.Models.ViewModels;
using Stripe;
using Stripe.Checkout;

namespace MotiZone.Services;

public class StripeService
{
    private readonly IConfiguration _config;
    private readonly ILogger<StripeService> _logger;

    public StripeService(IConfiguration config, ILogger<StripeService> logger)
    {
        _config = config;
        _logger = logger;
        StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
    }

    public async Task<string> CreateCheckoutSessionAsync(int orderId, List<CartViewModel> cartItems, string successUrl, string cancelUrl)
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = cartItems.Select(item => new SessionLineItemOptions {
                PriceData = new SessionLineItemPriceDataOptions {
                    Currency = "kes",
                    UnitAmount = (long)(item.Price * 100),
                    ProductData = new SessionLineItemPriceDataProductDataOptions {
                        Name = item.Name,
                        Metadata = new Dictionary<string, string> { ["orderId"] = orderId.ToString() }
                    }
                },
                Quantity = item.Quantity
            }).ToList(),
            Mode = "payment",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl,
            ClientReferenceId = orderId.ToString()
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);
        return session.Url;
    }
}