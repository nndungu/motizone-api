using System.ComponentModel.DataAnnotations;

namespace MotiZone.Models;

public class Part
{
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } 
    public string Description { get; set; }
    
    [Required] 
    public string Category { get; set; }
    
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
    
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; }
}