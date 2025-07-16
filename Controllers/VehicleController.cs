using Microsoft.AspNetCore.Mvc;
using MotiZone.Data;
using MotiZone.Models;
using MotiZone.Models.ViewModels;
using MotiZone.Services;

namespace MotiZone.Controllers;

public class VehiclesController : Controller
{
    private readonly VehicleService _vehicleService;
    private readonly ImageService _imageService;
    private readonly ApplicationDbContext _context;

    public VehiclesController(ApplicationDbContext context,VehicleService vehicleService, ImageService imageService)
    {
        _vehicleService = vehicleService;
        _imageService = imageService;
        _context = context;
    }

    public async Task<IActionResult> Index(string? make, string? model, int? year)
    {
        var vehicles = await _vehicleService.GetFilteredVehiclesAsync(make, model, year);
        var images = _imageService.GetImagesFromFolder("~/images/vehicles/car-placeholder.jpg");
        ViewBag.Images = images;
        var viewModel = new VehicleViewModel
        {
            Make = make,
            Model = model,
            Year = year,
            Vehicles = vehicles
        };

        return View(viewModel);
    }
    [HttpPost]
    public async Task<IActionResult> Create(Vehicle vehicle, IFormFile imageFile)
    {
        if (!ModelState.IsValid) return View(vehicle);

        vehicle.ImageUrl = await _imageService.SaveImageAsync(imageFile, "vehicles");

        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Details(int id)
    {
        var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
        if (vehicle == null) return NotFound();

        return View(vehicle);
    }
    
}