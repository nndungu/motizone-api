namespace MotiZone.Models;

public class Vehicle
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Make { get; set; } = null!;
    public string Model { get; set; } = null!;
    public int Year { get; set; }
    public decimal Price { get; set; }
    public decimal DailyRate { get; set; }
    public string? ImageUrl { get; set; }
    public string? Region { get; set; }
    public string Type { get; set; }
    public int Capacity { get; set; }
    public bool IsAvailableForRent { get; set; }
    public bool IsAvailableForSale { get; set; }
}