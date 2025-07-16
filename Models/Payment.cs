using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotiZone.Models;

public class Payment
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    
    [ForeignKey("UserId")]
    public User User { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; }

    // Optional references
    public string PaymentMethod { get; set; } 
    public string? TransactionId { get; set; }
}
