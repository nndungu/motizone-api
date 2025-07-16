namespace MotiZone.Models;

public enum OrderStatus
{
    Pending,
    Processed,
    Dispatched,
    Delivered,
    Cancelled
}
public class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; set; } 
    public string? DeliveryAddress { get; set; } 
    public string? PaymentMethod { get; set; }
    public string? PaymentStatus { get; set; }
    public string? TransactionId { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();     
}