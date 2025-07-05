using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MotiZone.Models;
using MotiZone.PdfDocuments;
using MotiZone.Services;
using QuestPDF.Fluent;

namespace MotiZone.Controllers;

public class RentalsController : Controller
{
    private readonly RentalService _rentalService;
    private readonly ImageService _imageService;

    public RentalsController(RentalService rentalService, ImageService imageService)
    {
        _rentalService = rentalService;
        _imageService = imageService;
    }

    // ===========================
    // USER VIEWS
    // ===========================

    public async Task<IActionResult> Index()
    {
        var rentals = await _rentalService.GetAllRentalsAsync();
        var images = _imageService.GetImagesFromFolder("~/images/shop/shop.jpg");
        ViewBag.Images = images;
        return View(rentals);
    }

    public async Task<IActionResult> Details(int id)
    {
        var rental = await _rentalService.GetRentalByIdAsync(id);
        return rental == null ? NotFound() : View(rental);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(int id)
    {
        var rental = await _rentalService.GetRentalByIdAsync(id);
        if (rental == null) return NotFound();

        await _rentalService.BookRentalAsync(rental);
        return RedirectToAction(nameof(Index));
    }

    // ===========================
    // ADMIN VIEWS
    // ===========================

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminList()
    {
        var rentals = await _rentalService.GetAllRentalsAsync();
        return View(rentals);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        await PopulateVehiclesDropdown();
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Rental rental, IFormFile? imageFile)
    {
        if (!ModelState.IsValid)
        {
            await PopulateVehiclesDropdown();
            return View(rental);
        }

        if (imageFile != null && imageFile.Length > 0)
        {
            var fileName = Path.GetFileName(imageFile.FileName);
            var imagePath = Path.Combine("wwwroot/images/rentals/rental-car.jpg", fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(imagePath)!); // Ensure directory exists

            using var stream = new FileStream(imagePath, FileMode.Create);
            await imageFile.CopyToAsync(stream);

            rental.ImageUrl = $"/images/rentals/{fileName}";
        }

        await _rentalService.AddRentalAsync(rental);
        return RedirectToAction(nameof(AdminList));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var rental = await _rentalService.GetRentalByIdAsync(id);
        if (rental == null) return NotFound();

        await PopulateVehiclesDropdown();
        return View(rental);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Rental rental)
    {
        if (!ModelState.IsValid)
        {
            await PopulateVehiclesDropdown();
            return View(rental);
        }

        await _rentalService.UpdateRentalAsync(rental);
        return RedirectToAction(nameof(AdminList));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelRental(int id)
    {
        var result = await _rentalService.CancelRentalAsync(id);
        return result ? RedirectToAction(nameof(AdminList)) : NotFound();
    }

    // ===========================
    // HELPERS
    // ===========================

    private async Task PopulateVehiclesDropdown()
    {
        var vehicles = await _rentalService.GetAllRentalsAsync();
        ViewBag.Vehicles = new SelectList(vehicles, "Id", "Model");
    }

    // ===========================
    // TODO: Invoice PDF generation
    // ===========================
    [Authorize]
    public async Task<IActionResult> Invoice(int id)
    {
        var rental = await _rentalService.GetRentalByIdAsync(id);
        if (rental == null) return NotFound();

        var document = new RentalInvoiceDocument(rental);
        var pdfBytes = document.GeneratePdf();

        return File(pdfBytes, "application/pdf", $"Rental_Invoice_{id}.pdf");
    }

}
