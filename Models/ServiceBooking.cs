namespace MotiZone.Models;

public class ServiceBooking
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public int ServiceId { get; set; }

    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }

    public Service? Service { get; set; }
}