using Microsoft.EntityFrameworkCore;
using MotiZone.Data;
using MotiZone.Models;
using MotiZone.Models.ViewModels;

namespace MotiZone.Services;

public class CartService
{
    private readonly ApplicationDbContext _context;

    public CartService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CartViewModel>> GetCartItemsForDisplayAsync(int userId)
    {
        return await _context.CartItems
            .Where(ci => ci.UserId == userId)
            .Select(ci => new CartViewModel
            {
                Id = ci.Id,
                Name = ci.Part != null
                    ? ci.Part.Name
                    : ci.Vehicle != null
                        ? $"{ci.Vehicle.Make} {ci.Vehicle.Model} ({ci.Vehicle.Year})"
                        : "Unknown Item", 
                Type = ci.Part != null
                    ? "Part"
                    : ci.Vehicle != null
                        ? "Vehicle"
                        : ci.Customization != null
                            ? "Customization"
                            : "Unknown",
                Quantity = ci.Quantity,
                Price = ci.Part != null
                    ? ci.Part.Price
                    : ci.Vehicle != null
                        ? ci.Vehicle.Price
                        : 0
            })
            .OrderBy(ci => ci.Id)
            .ToListAsync();
    }

    public async Task AddVehicleToCartAsync(int vehicleId, int userId)
    {
        var cartItem = new CartItem
        {
            UserId = userId,
            VehicleId = vehicleId,
            Quantity = 1,
            DateAdded = DateTime.Now
        };

        _context.CartItems.Add(cartItem);
        await _context.SaveChangesAsync();
    }

    public async Task AddPartToCartAsync(int partId, int userId)
    {
        var cartItem = new CartItem
        {
            UserId = userId,
            PartId = partId,
            Quantity = 1,
            DateAdded = DateTime.Now
        };

        _context.CartItems.Add(cartItem);
        await _context.SaveChangesAsync();
    }
    public async Task AddCustomizationToCartAsync(int customizationId, int userId)
    {
        var cartItem = new CartItem
        {
            UserId = userId,
            CustomizationId = customizationId,
            Quantity = 1,
            DateAdded = DateTime.Now
        };

        _context.CartItems.Add(cartItem);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveFromCartAsync(int id)
    {
        var cartItem = await _context.CartItems.FindAsync(id);
        if (cartItem != null)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }
    }

    public async Task ClearCartAsync(int userId)
    {
        var items = await _context.CartItems
            .Where(ci => ci.UserId == userId)
            .ToListAsync();

        _context.CartItems.RemoveRange(items);
        await _context.SaveChangesAsync();
    }
}
