using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotiZone.Services;

namespace MotiZone.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("Admin/[controller]/[action]")]
public class AdminRentalsController : Controller
{
    private readonly RentalService _rentalService;

    public AdminRentalsController(RentalService rentalService)
    {
        _rentalService = rentalService;
    }

    public async Task<IActionResult> Index()
    {
        var rentals = await _rentalService.GetAllRentalsAsync();
        return View(rentals);
    }
}