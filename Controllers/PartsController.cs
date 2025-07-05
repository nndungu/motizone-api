using Microsoft.AspNetCore.Mvc;
using MotiZone.Models;
using MotiZone.Models.ViewModels;
using MotiZone.Services;

namespace MotiZone.Controllers;

public class PartsController : Controller
{
    private readonly PartService _partService;
    private readonly ImageService _imageService;

    public PartsController(PartService partService, ImageService imageService)
    {
        _partService = partService;
        _imageService = imageService;
    }

    // MVC View (Shop page)
    [HttpGet]
    public IActionResult Index(string? name, string? category)
    {
        var filteredParts = _partService.GetFilteredPartsAsync(name, category).Result;
        var images = _imageService.GetImagesFromFolder("~/images/shop/shop.jpg");
        ViewBag.Images = images;

        var viewModel = new PartListViewModel
        {
            Name = name,
            Category = category,
            Parts = filteredParts
        };

        return View(viewModel);
    }
    
    public async Task<IActionResult> GetAllParts()
    {
        var parts = await _partService.GetPartsAsync();
        return Ok(parts);
    }
    public async Task<IActionResult> Get(int id)
    {
        var part = await _partService.GetPartByIdAsync(id);
        if (part == null) return NotFound();
        return Ok(part);
    }
    public async Task<ActionResult<Part>> Create([FromBody] Part part)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var created = await _partService.AddPartAsync(part);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }
    
    public async Task<IActionResult> Edit(int id)
    {
        var deleted = await _partService.DeletePartAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}