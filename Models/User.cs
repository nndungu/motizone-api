using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MotiZone.Models;

public class User  : IdentityUser<int>
{
    public int Id { get; set; }
    public new string? UserName { get; set; } 
    [Required] public string Role { get; set; }  
    public bool IsAdmin { get; set; }
    public bool IsActive { get; set; }
    public new string? Email { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}
