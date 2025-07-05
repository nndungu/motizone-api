using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MotiZone.Models;
using MotiZone.Services;

namespace MotiZone.Controllers;

[Authorize]
public class PaymentController : Controller
{
    private readonly CartService _cartService;
    private readonly PaymentService _paymentService;
    private readonly UserManager<User> _userManager;

    public PaymentController(CartService cartService, PaymentService paymentService, UserManager<User> userManager)
    {
        _cartService = cartService;
        _paymentService = paymentService;
        _userManager = userManager;
    }

    private int? GetUserId()
    {
        var userIdStr = _userManager.GetUserId(User);
        return int.TryParse(userIdStr, out int id) ? id : null;
    }

    public async Task<IActionResult> Checkout()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var cartItems = await _cartService.GetCartItemsForDisplayAsync(userId.Value);
        var total = cartItems.Sum(i => i.Price * i.Quantity);

        return View(model: total);
    }

    [HttpPost]
    public async Task<IActionResult> Confirm()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var cartItems = await _cartService.GetCartItemsForDisplayAsync(userId.Value);
        var total = cartItems.Sum(i => i.Price * i.Quantity);

        if (total <= 0) return BadRequest("Cart is empty.");

        await _paymentService.CreatePaymentAsync(userId.Value, total, "Cash"); // or "Card"
        await _cartService.ClearCartAsync(userId.Value);

        return RedirectToAction("Receipt");
    }

    public async Task<IActionResult> Receipt()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var payments = await _paymentService.GetPaymentsByUserAsync(userId.Value);
        return View(payments);
    }
}