namespace MotiZone.Models;

public class Payment
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; }

    // Optional references
    public string PaymentMethod { get; set; } = "Card";
    public string? TransactionId { get; set; }
}
