using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MotiZone.Models;
using MotiZone.Services;

namespace MotiZone.Controllers;

[Authorize]
public class ServicesController : Controller
{
    private readonly ServiceService _serviceService;
    private readonly ServiceBookingService _bookingService;
    private readonly UserManager<User> _userManager;

    public ServicesController(ServiceService serviceService, ServiceBookingService bookingService, UserManager<User> userManager)
    {
        _serviceService = serviceService;
        _bookingService = bookingService;
        _userManager = userManager;
    }

    // ------------------------
    // USER: View & Book Services
    // ------------------------

    public async Task<IActionResult> Index()
    {
        var services = await _serviceService.GetAllAsync();
        return View(services);
    }

    public async Task<IActionResult> Details(int id)
    {
        var service = await _serviceService.GetByIdAsync(id);
        return service == null ? NotFound() : View(service);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(int id, string notes)
    {
        var service = await _serviceService.GetByIdAsync(id);
        if (service == null) return NotFound();

        var userIdStr = _userManager.GetUserId(User);
        if (!int.TryParse(userIdStr, out var userId))
        {
            return Unauthorized(); // Invalid user ID
        }

        var booking = new ServiceBooking
        {
            ServiceId = id,
            UserId = userId,
            Notes = notes
        };

        await _bookingService.AddBookingAsync(booking);
        return RedirectToAction(nameof(Index));
    }

    // ------------------------
    // ADMIN: Manage Services
    // ------------------------

    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View();

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Service service)
    {
        if (!ModelState.IsValid) return View(service);
        await _serviceService.AddAsync(service);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var service = await _serviceService.GetByIdAsync(id);
        return service == null ? NotFound() : View(service);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Service service)
    {
        if (!ModelState.IsValid) return View(service);
        await _serviceService.UpdateAsync(service);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var service = await _serviceService.GetByIdAsync(id);
        return service == null ? NotFound() : View(service);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmDelete(int id)
    {
        await _serviceService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
