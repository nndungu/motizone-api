// Models/Order.cs
using System.ComponentModel.DataAnnotations;

namespace MotiZone.Models;

public class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = "Pending";

    public List<CartItem> Items { get; set; } = new();
}