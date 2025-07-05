namespace MotiZone.Models;

public class Rental
{
    public int Id { get; set; }
    public int UserId { get; set; } // foreign key property
    public User? User { get; set; } // navigatioo property
    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; }
    public string VehicleName { get; set; } 
    public string Status { get; set; }
    public bool IsActive { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DailyRate { get; set; }
    public string DurationDays { get; set; }
    public decimal TotalCost { get; set; }
    public string? ImageUrl { get; set; }

}