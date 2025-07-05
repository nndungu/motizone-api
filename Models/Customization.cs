using System.ComponentModel.DataAnnotations;

namespace MotiZone.Models;

public class Customization
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;
    public string Type { get; set; } = string.Empty;

    public string? Description { get; set; }

    [DataType(DataType.Currency)]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }
    
}