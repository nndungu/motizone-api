using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotiZone.Models;
using MotiZone.Services;

namespace MotiZone.Controllers;

public class CustomizationController : Controller
{
    private readonly CustomizationService _customizationService;

    public CustomizationController(CustomizationService customizationService)
    {
        _customizationService = customizationService;
    }

    // Public: View all customizations
    public async Task<IActionResult> Index()
    {
        var customization = await _customizationService.GetAvailableCustomizationsAsync();
        return Ok(customization);
    }

    // Public: View a single customization
    public async Task<IActionResult> Details(int id)
    {
        var customization = await _customizationService.GetCustomizationByIdAsync(id);
        return customization == null ? NotFound() : View(customization);
    }

    // ========================
    // Admin Features (Optional)
    // ========================

    [Authorize(Roles = "Admin")]
    public IActionResult Create() => Ok();

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Customization customization)
    {
        if (!ModelState.IsValid) return Ok(customization);

        await _customizationService.AddCustomizationAsync(customization);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var customization = await _customizationService.GetCustomizationByIdAsync(id);
        return customization == null ? NotFound() : Ok(customization);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(Customization customization)
    {
        if (!ModelState.IsValid) return Ok(customization);

        await _customizationService.UpdateCustomizationAsync(customization);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var customization = await _customizationService.GetCustomizationByIdAsync(id);
        return customization == null ? NotFound() : Ok(customization);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ConfirmDelete(int id)
    {
        await _customizationService.DeleteCustomizationAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
