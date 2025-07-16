namespace MotiZone.Models.ViewModels;

public class AdminPaymentViewModel
{
    public int Id { get; set; }
    public string Method { get; set; } // Stripe or Mpesa
    public string Status { get; set; }
    public string Reference { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string UserEmail { get; set; }
}