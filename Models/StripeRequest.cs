namespace MotiZone.Models;

public class StripeRequest
{
    public string ProductName { get; set; }
    public long AmountCents { get; set; }
    public string SuccessUrl { get; set; }
    public string CancelUrl { get; set; }
}