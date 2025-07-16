namespace MotiZone.Models.ViewModels;

public class CartViewModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }

    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal TotalAmount => Quantity * Price;
    public string ImageUrl { get; set; }
}
    