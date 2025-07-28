namespace MotiZone.Models.ViewModels;

public class CustomizationViewModel
{
    public string? Type { get; set; }
    public string? Description { get; set; }
    
    public List<Customization> Items { get; set; } = new();
    
}