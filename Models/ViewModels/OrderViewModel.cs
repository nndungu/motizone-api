// ViewModels/OrderViewModel.cs

using MotiZone.Models;

namespace MotiZone.Models.ViewModels;

public class OrderViewModel
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public List<CartItem> Items { get; set; } = new();
}