namespace MotiZone.Models;

public class MpesaTransaction
{
    public int Id { get; set; }
    public string TransactionId { get; set; }
    public string PhoneNumber { get; set; }
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; }
    public string Status { get; set; }
}