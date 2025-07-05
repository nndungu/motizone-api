using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotiZone.Models;
using MotiZone.Services;

namespace MotiZone.Controllers;

[Authorize(Roles = "Admin")]
public class AdminVehiclesController : Controller
{
    private readonly VehicleService _vehicleService;
    private readonly IWebHostEnvironment _env;

    public AdminVehiclesController(VehicleService vehicleService, IWebHostEnvironment env)
    {
        _vehicleService = vehicleService;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var vehicles = await _vehicleService.GetAllVehiclesAsync();
        return View(vehicles);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Vehicle vehicle, IFormFile ImageFile)
    {
        if (ModelState.IsValid)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                var uploadPath = Path.Combine(_env.WebRootPath, "images", "vehicles");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                vehicle.ImageUrl = $"images/vehicles/{fileName}";
            }

            await _vehicleService.AddVehicleAsync(vehicle);
            return RedirectToAction(nameof(Index));
        }

        return View(vehicle);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
        return vehicle == null ? NotFound() : View(vehicle);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Vehicle vehicle)
    {
        if (!ModelState.IsValid) return View(vehicle);

        await _vehicleService.UpdateVehicleAsync(vehicle);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(int id)
    {
        var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
        return vehicle == null ? NotFound() : View(vehicle);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _vehicleService.DeleteVehicleAsync(id);
        return RedirectToAction("Index");
    }
}
