namespace MotiZone.Models.ViewModels;

public class PartListViewModel
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public decimal? Price { get; set; }
    
    public List<Part> Parts { get; set; } = new();
}