namespace MotiZone.Models;

public class FinancingApplication
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public decimal RequestedAmount { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime SubmittedAt { get; set; } = DateTime.Now;
}