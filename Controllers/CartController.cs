using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MotiZone.Models;
using MotiZone.Models.ViewModels;
using MotiZone.Services;

namespace MotiZone.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly CartService _cartService;
    private readonly CustomizationService _customizationService;
    private readonly UserManager<User> _userManager;

    public CartController(CartService cartService, CustomizationService customizationService, UserManager<User> userManager)
    {
        _cartService = cartService;
        _customizationService = customizationService;
        _userManager = userManager;
    }

    private int? GetUserId()
    {
        var userIdStr = _userManager.GetUserId(User);
        return int.TryParse(userIdStr, out int id) ? id : null;
    }

    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var items = await _cartService.GetCartItemsForDisplayAsync(userId.Value);
        return View(items);
    }

    [HttpPost]
    public async Task<IActionResult> AddVehicle(int vehicleId)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        await _cartService.AddVehicleToCartAsync(vehicleId, userId.Value);
        TempData["Message"] = "Vehicle added to cart.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> AddPart(int partId)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        await _cartService.AddPartToCartAsync(partId, userId.Value);
        TempData["Message"] = "Part added to cart.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> AddCustomization(int customizationId)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        await _cartService.AddCustomizationToCartAsync(customizationId, userId.Value);
        TempData["Message"] = "Customization added to cart.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int id)
    {
        await _cartService.RemoveFromCartAsync(id);
        TempData["Message"] = "Item removed from cart.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Clear()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        await _cartService.ClearCartAsync(userId.Value);
        TempData["Message"] = "Cart cleared.";
        return RedirectToAction(nameof(Index));
    }
}
