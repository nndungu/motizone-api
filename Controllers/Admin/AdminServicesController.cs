using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotiZone.Data;
using MotiZone.Models;

namespace MotiZone.Controllers.Admin;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("Admin/[controller]/[action]")]
public class AdminServicesController : Controller
{
    private readonly ApplicationDbContext _context;
    public AdminServicesController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> 
        Index() => View(await _context.Services.ToListAsync());

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(Service service)
    {
        if (ModelState.IsValid)
        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(service);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var service = await _context.Services.FindAsync(id);
        return service == null ? NotFound() : View(service);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Service service)
    {
        if (ModelState.IsValid)
        {
            _context.Update(service);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(service);
    }
    
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var service = await _context.Services.FindAsync(id);
        return service == null ? NotFound() : View(service);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var service = await _context.Services.FindAsync(id);
        if (service != null)
        {
            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}