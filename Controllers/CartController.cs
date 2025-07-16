using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotiZone.Data;
using MotiZone.Models;
using MotiZone.Models.ViewModels;
using MotiZone.Services;

namespace MotiZone.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly CartService _cartService;
    private readonly CustomizationService _customizationService;
    private readonly UserManager<User> _userManager;

    public CartController(ApplicationDbContext context, CartService cartService, CustomizationService customizationService, UserManager<User> userManager)
    {
        _context = context;
        _cartService = cartService;
        _customizationService = customizationService;
        _userManager = userManager;
    }

    private async Task<int?> GetUserIdAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        return user?.Id;
    }

    public async Task<IActionResult> Index()
    {
        var userId = await GetUserIdAsync();
        if (userId == null) return Unauthorized();

        var items = await _cartService.GetCartItemsForDisplayAsync(userId.Value);
        return View(items);
    }

    [HttpPost]
    public async Task<IActionResult> AddVehicle(int vehicleId)
    {
        var userId = await GetUserIdAsync();
        if (userId == null) return Unauthorized();

        await _cartService.AddVehicleToCartAsync(vehicleId, userId.Value);
       // TempData["Message"] = "Vehicle added to cart.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> AddPart(int partId)
    {
        var userId = await GetUserIdAsync();
        if (userId == null) return Unauthorized();

        await _cartService.AddPartToCartAsync(partId, userId.Value);
        //TempData["Message"] = "Part added to cart.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> AddCustomization(int customizationId)
    {
        var userId = await GetUserIdAsync();
        if (userId == null) return Unauthorized();

        await _cartService.AddCustomizationToCartAsync(customizationId, userId.Value);
        //TempData["Message"] = "Customization added to cart.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int id)
    {
        await _cartService.RemoveFromCartAsync(id);
       // TempData["Message"] = "Item removed from cart.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Clear()
    {
        var userId = await GetUserIdAsync();
        if (userId == null) return Unauthorized();

        await _cartService.ClearCartAsync(userId.Value);
        //TempData["Message"] = "Cart cleared.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize]
    public IActionResult Checkout()
    {
        return View();
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Checkout(CheckoutViewModel model)
    {
        var userId = _userManager.GetUserId(User);
        var cartItems = await _context.CartItems
            .Include(c => c.Vehicle)
            .Include(c => c.Part)
            .Include(c => c.Customization)
            .Where(c => c.UserId.ToString() == userId)
            .ToListAsync();

        if (!cartItems.Any())
            return RedirectToAction("Index");

        var order = new Order
        {
            UserId = int.Parse(userId),
            OrderDate = DateTime.UtcNow,
            TotalAmount = cartItems.Sum(item =>
                (item.Vehicle?.Price ?? 0) +
                (item.Part?.Price ?? 0) +
                (item.Customization?.Price ?? 0)
            ),
            DeliveryAddress = model.DeliveryAddress,
            PaymentMethod = model.PaymentMethod,
            Items = cartItems.Select(ci => new OrderItem
            {
                VehicleId = ci.VehicleId,
                PartId = ci.PartId,
                CustomizationId = ci.CustomizationId,
                Quantity = ci.Quantity
            }).ToList()
        };

        _context.Orders.Add(order);
        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();
        
       //await SendOrderConfirmationEmail(order);

       return RedirectToAction("Details", "Order", new { id = order.Id });
    }
    
    [HttpPost]
    public IActionResult Checkout(string method, decimal amount)
    {
        if (method == "stripe")
            return RedirectToAction("Checkout", "Payment", new { amount });
        else
            return RedirectToAction("Pay", "Mpesa", new { phoneNumber = "2547XXXXXXXX", amount });
    }

}
