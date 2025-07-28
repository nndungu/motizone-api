using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotiZone.Data;
using MotiZone.Models;

namespace MotiZone.Controllers.Admin;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("Admin/[controller]/[action]")]
public class AdminCustomizationsController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminCustomizationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var customizations = await _context.Customizations.ToListAsync();
        return View(customizations);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var customization = await _context.Customizations.FirstOrDefaultAsync(m => m.Id == id);
        if (customization == null) return NotFound();

        return View(customization);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var customization = await _context.Customizations.FindAsync(id);
        if (customization == null) return NotFound();

        return View(customization);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Customization customization)
    {
        if (id != customization.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(customization);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Customizations.Any(e => e.Id == customization.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(customization);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var customization = await _context.Customizations
            .FirstOrDefaultAsync(m => m.Id == id);
        if (customization == null) return NotFound();

        return View(customization);
    }

    [HttpPost,]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var customization = await _context.Customizations.FindAsync(id);
        if (customization != null)
        {
            _context.Customizations.Remove(customization);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
