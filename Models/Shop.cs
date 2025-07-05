namespace MotiZone.Models;

public class Shop
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public string ContactEmail { get; set; }
    
    //Navigation
    public List<Vehicle> Vehicles { get; set; } = new();
    public List<Part> Parts { get; set; } = new();
}