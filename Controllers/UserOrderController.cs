using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MotiZone.Models;
using MotiZone.Services;

namespace MotiZone.Controllers;

[Authorize]
public class UserOrdersController : Controller
{
    private readonly OrderService _orderService;
    private readonly UserManager<User> _userManager;

    public UserOrdersController(OrderService orderService, UserManager<User> userManager)
    {
        _orderService = orderService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge(); // forces login
        }

        var orders = await _orderService.GetUserOrdersAsync(user.Id);
        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        var orders = await _orderService.GetUserOrdersAsync(user.Id);
        var order = orders.FirstOrDefault(o => o.Id == id);
        return order == null ? NotFound() : View(order);
    }
}