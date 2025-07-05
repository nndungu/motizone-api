using Microsoft.AspNetCore.Mvc;
using MotiZone.Models.ViewModels;
using MotiZone.Services;

namespace MotiZone.Controllers;

public class VehiclesController : Controller
{
    private readonly VehicleService _vehicleService;
    private readonly ImageService _imageService;

    public VehiclesController(VehicleService vehicleService, ImageService imageService)
    {
        _vehicleService = vehicleService;
        _imageService = imageService;
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
    public async Task<IActionResult> Details(int id)
    {
        var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
        if (vehicle == null) return NotFound();

        return View(vehicle);
    }
    
}