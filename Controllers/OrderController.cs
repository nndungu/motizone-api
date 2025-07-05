using Microsoft.AspNetCore.Mvc;
using MotiZone.Services;
using MotiZone.Models.ViewModels;

public class OrdersController : Controller
{
    private readonly OrderService _orderService;

    public OrdersController(OrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IActionResult> Index()
    {
        int userId = 1; // Replace with real user context
        var orders = await _orderService.GetUserOrdersAsync(userId);

        var viewModels = orders.Select(o => new OrderViewModel
        {
            Id = o.Id,
            OrderDate = o.OrderDate,
            TotalAmount = o.TotalAmount,
            Items = o.Items
        }).ToList();

        return View(viewModels);
    }
}