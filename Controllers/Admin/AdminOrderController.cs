// Controllers/AdminOrdersController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotiZone.Data;
using MotiZone.Models;
using MotiZone.Models.ViewModels;
using MotiZone.Services;

namespace MotiZone.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class AdminOrdersController : Controller
{
    private readonly ApplicationDbContext _context;
    public AdminOrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var orders = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Items)
            .ToListAsync();
        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _context.Orders 
            .Include(o => o.User)
            .Include(o => o.Items)
            .ThenInclude(i => i.Vehicle)
            .Include(o => o.Items)
            .ThenInclude(i => i.Part)
            .Include(o => o.Items)
            .ThenInclude(i => i.Customization)
            .FirstOrDefaultAsync(o => o.Id == id);
        
        if (order == null)
            return NotFound();

        return View(order);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null ) return NotFound();
        
        order.Status = status;
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", new { id = id });
    }
}