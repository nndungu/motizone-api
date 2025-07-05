// Services/OrderService.cs
using Microsoft.EntityFrameworkCore;
using MotiZone.Data;
using MotiZone.Models;

namespace MotiZone.Services;

public class OrderService
{
    private readonly ApplicationDbContext _context;
    private readonly CartService _cartService;

    public OrderService(ApplicationDbContext context, CartService cartService)
    {
        _context = context;
        _cartService = cartService;
    }

    public async Task<Order> CheckoutAsync(int userId)
    {
        var cartItems = await _context.CartItems
            .Include(ci => ci.Vehicle)
            .Include(ci => ci.Part)
            .Where(ci => ci.UserId == userId)
            .ToListAsync();

        if (!cartItems.Any()) throw new Exception("Cart is empty");

        var order = new Order
        {
            UserId = userId,
            TotalAmount = cartItems.Sum(i =>
                (i.Vehicle?.Price ?? 0) + (i.Part?.Price ?? 0))
        };

        foreach (var item in cartItems)
        {
            order.Items.Add(new CartItem
            {
                VehicleId = item.VehicleId,
                PartId = item.PartId,
                Quantity = item.Quantity,
                Price = (item.Vehicle?.Price ?? 0) + (item.Part?.Price ?? 0)
            });
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        await _cartService.ClearCartAsync(userId);
        return order;
    }

    public async Task<List<Order>> GetUserOrdersAsync(int userId)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Vehicle)
            .Include(o => o.Items)
            .ThenInclude(i => i.Part)
            .Where(o => o.UserId == userId)
            .ToListAsync();
    }
    public async Task<List<Order>> GetAllOrdersAsync()
    {
        return await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Vehicle)
            .Include(o => o.Items)
            .ThenInclude(i => i.Part)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Vehicle)
            .Include(o => o.Items)
            .ThenInclude(i => i.Part)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<bool> UpdateOrderStatusAsync(int id, string status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return false;

        order.Status = status;
        await _context.SaveChangesAsync();
        return true;
    }

}