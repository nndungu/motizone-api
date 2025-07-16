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
            .Include(ci => ci.Vehicle)
            .Include(ci => ci.Part)
            .Include(ci => ci.Customization)
            .Select(ci => new CartViewModel
            {
                Id = ci.Id,
                Name = ci.Part != null
                    ? ci.Part.Name
                    : ci.Vehicle != null
                        ? $"{ci.Vehicle.Make} {ci.Vehicle.Model} ({ci.Vehicle.Year})"
                        : ci.Customization != null
                        ? ci.Customization.Name 
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
                        : ci.Customization != null
                        ? ci.Customization.Price
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

    public async Task<int?> CheckoutAsync(int userId)
    {
        var cartItems = await _context.CartItems
            .Where(ci => ci.UserId == userId)
            .Include(ci => ci.Part)
            .Include(ci => ci.Customization)
            .Include(ci => ci.Vehicle)
            .ToListAsync();
            
            if(!cartItems.Any())
                return null;

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = cartItems.Sum(ci =>
                    ci.Part?.Price ?? ci.Vehicle?.Price ?? ci.Customization?.Price ?? 0)
            };
            
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    VehicleId = cartItem.VehicleId,
                    PartId = cartItem.PartId,
                    CustomizationId = cartItem.CustomizationId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Part?.Price ?? cartItem.Vehicle?.Price ?? cartItem.Customization?.Price ?? 0
                };
                _context.OrderItems.Add(orderItem);
            }
            
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            
            return order.Id;
    }
}
