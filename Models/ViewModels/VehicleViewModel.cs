namespace MotiZone.Models.ViewModels;

public class VehicleViewModel
{
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }

    public List<Vehicle> Vehicles { get; set; } = new();
}