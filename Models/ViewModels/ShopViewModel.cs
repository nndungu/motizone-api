namespace MotiZone.Models.ViewModels;

public class ShopViewModel
{
    public int? ShopId { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
    
    public List<Vehicle> Vehicles { get; set; }
    public List<Part> Parts { get; set; }
}