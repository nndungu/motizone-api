using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotiZone.Data;
using MotiZone.Models;
using MotiZone.Services;

namespace MotiZone.Controllers;

[Route("admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("payments")]
    public async Task<IActionResult> Payments()
    {
        var payments = await _context.Payments
            .Include(p => p.User)
            .OrderByDescending(p => p.PaidAt)
            .ToListAsync();

        return View(payments);
    }

    [HttpGet("orders")]
    public async Task<IActionResult> Orders()
    {
        var orders = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Vehicle)
            .Include(o => o.Items)
            .ThenInclude(i => i.Part)
            .Include(o => o.Items)
            .ThenInclude(i => i.Customization)
            .Include(o => o.User)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return View(orders);
    }

    [HttpGet("receipts/{id}")]
    public async Task<IActionResult> Receipt(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Part)
            .Include(o => o.Items)
            .ThenInclude(i => i.Vehicle)
            .Include(o => o.Items)
            .ThenInclude(i => i.Customization)
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return NotFound();

        var pdfService = new PdfInvoiceService();
        var pdf = pdfService.GenerateInvoice(order);

        return File(pdf, "application/pdf", $"Invoice_{order.Id}.pdf");
    }
}