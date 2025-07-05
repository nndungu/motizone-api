namespace MotiZone.Models;

public class ServiceLog
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public DateTime ServiceDate { get; set; }
    public string Description { get; set; }
}