using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotiZone.Data;
using MotiZone.Models;
using MotiZone.Services;
using MotiZone.Models.ViewModels;

[ApiController]
[Route("orders/[action]")]
[Authorize]
public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;
    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
        
        var orders = await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.Items)
            .ThenInclude(i => i.Part)
            .Include(o => o.Items)
            .ThenInclude(i => i.Vehicle)
            .Include(o => o.Items)
            .ThenInclude(i => i.Customization)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
        
        return View();
    }
    
    [HttpGet]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> Details(int? id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Part)
            .Include(o => o.Items)
            .ThenInclude(i => i.Customization)
            .Include(o => o.Items)
            .ThenInclude(i => i.Vehicle)
            .Include(o => o.User)
            .FirstOrDefaultAsync(m => m.Id == id);
        
        if (order == null || (User.IsInRole("Customer") && order.User.Email != User.Identity.Name))
            return NotFound();
        
        return View(order);
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> All()
    {
        var orders = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Items)
            .ThenInclude(i => i.Vehicle)
            .Include(o => o.Items)
            .ThenInclude(i => i.Part)
            .Include(o => o.Items)
            .ThenInclude(i => i.Customization)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return View("AllOrders", orders);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
            return NotFound();
        
        order.Status = status;
        await _context.SaveChangesAsync();
        return RedirectToAction("All");
    }
    /*public IActionResult ExportToPdf(int id)
    {
        var order = _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Vehicle)
            .Include(o => o.User)
            .FirstOrDefault(o => o.Id == id);

        return new ViewAsPdf("Invoice", order)
        {
            FileName = $"order-{id}.pdf"
        };
    }
    public async Task<IActionResult> ExportAllOrdersToExcel()
    {
        var orders = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Items)
            .ToListAsync();

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Orders");

        worksheet.Cells["A1"].Value = "Order ID";
        worksheet.Cells["B1"].Value = "User";
        worksheet.Cells["C1"].Value = "Total";
        worksheet.Cells["D1"].Value = "Date";

        for (int i = 0; i < orders.Count; i++)
        {
            var o = orders[i];
            worksheet.Cells[i + 2, 1].Value = o.Id;
            worksheet.Cells[i + 2, 2].Value = o.User.Email;
            worksheet.Cells[i + 2, 3].Value = o.TotalAmount;
            worksheet.Cells[i + 2, 4].Value = o.OrderDate.ToShortDateString();
        }

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Orders.xlsx");
    }*/
    

}