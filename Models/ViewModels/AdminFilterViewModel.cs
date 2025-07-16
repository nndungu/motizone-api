namespace MotiZone.Models.ViewModels;

public class AdminFilterViewModel
{
    public int? UserId { get; set; }
    public string? Method { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}