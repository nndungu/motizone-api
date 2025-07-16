namespace MotiZone.Models;

public class CartItem
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public int? VehicleId { get; set; }
    public int? PartId { get; set; }
    public int? CustomizationId { get; set; }
    public int? OrderId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTime DateAdded { get; set; }
    
    public Vehicle? Vehicle { get; set; }
    public Customization? Customization { get; set; }
    public Part? Part { get; set; }
    public Order? Order { get; set; }
    
}