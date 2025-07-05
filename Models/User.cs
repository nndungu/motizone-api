using Microsoft.AspNetCore.Identity;

namespace MotiZone.Models;

public class User : IdentityUser<int>
{
    // Avoid redefining `Id`, `Email`, or `PasswordHash`
    public string UserName { get; set; } // Optional: IdentityUser already has `UserName`
    public byte[]? PasswordSalt { get; set; } // Optional custom field
    public string Role { get; set; }  // Consider using Identity Roles instead
    public bool IsAdmin { get; set; }
    public bool IsActive { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}
