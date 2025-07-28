using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MotiZone.Models;
using MotiZone.Services;
using Stripe.Checkout;

namespace MotiZone.Controllers;

[ApiController]
[Route("payments/[action]")]
[Authorize]
public class PaymentController : Controller
{
    private readonly CartService _cartService;
    private readonly PaymentService _paymentService;
    private readonly UserManager<User> _userManager;

    public PaymentController(CartService cartService, UserManager<User> userManager)
    {
        _cartService = cartService;
       // _paymentService = paymentService;
        _userManager = userManager;
    }

    [HttpGet]
    private int? GetUserId()
    {
        var userIdStr = _userManager.GetUserId(User);
        return int.TryParse(userIdStr, out int id) ? id : null;
    }
    
    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var cartItems = await _cartService.GetCartItemsForDisplayAsync(userId.Value);
        var total = cartItems.Sum(i => i.Price * i.Quantity);

        return View(model: total);
    }
    
    [HttpPost]
    public async Task<IActionResult> StripeCheckout()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();
        
        var cartItems = await _cartService.GetCartItemsForDisplayAsync(userId.Value);
        var total = cartItems.Sum(i => i.Price * i.Quantity);

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = cartItems.Select(item => new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "usd",
                    UnitAmount = (long)(item.Price * 100),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Name
                    },
                },
                Quantity = item.Quantity,
            }).ToList(),
            Mode = "payment",
            SuccessUrl = Url.Action("Success", "Payments", null, Request.Scheme),
            CancelUrl = Url.Action("Cancel", "Payments", null, Request.Scheme),
        };

        var service = new SessionService();
        Session session = await service.CreateAsync(options);

        return Redirect(session.Url);
    }
    
    [HttpGet]
    public IActionResult Success()
    {
        ViewBag.Message = "Payment successful! Thank you for your purchase.";
        return View();
    }
    
    [HttpGet]
    public IActionResult Cancel()
    {
        ViewBag.Message = "Payment cancelled! Please try again.";
        return View();
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

    [HttpGet]
    public async Task<IActionResult> Receipt()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var payments = await _paymentService.GetPaymentsByUserAsync(userId.Value);
        return View(payments);
    }
    
}