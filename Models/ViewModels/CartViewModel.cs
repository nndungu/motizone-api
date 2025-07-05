namespace MotiZone.Models.ViewModels;

public class CartViewModel
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string? Name { get; set; }
    public string? Type {get; set;}
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    
    public List<Vehicle> Vehicles { get; set; }
    public List<Part> Parts { get; set; }
    public List<Customization> Customizations { get; set; }


    // For display purposes (not stored in DB)
    /*
    [NotMapped]
    public string Name { get; set; }

    [NotMapped]
    public decimal Price { get; set; }

    [NotMapped]
    public string Type { get; set; }
    */

}