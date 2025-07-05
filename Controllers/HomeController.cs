using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MotiZone.Models;

namespace MotiZone.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // Landing Page
    public IActionResult Index()
    {
        _logger.LogInformation("User visited Home/Index at {Time}", DateTime.UtcNow);
        return View();
    }

    // Privacy Policy Page
    public IActionResult Privacy()
    {
        return View();
    }

    // Error Page
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var errorModel = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };

        _logger.LogError("An error occurred. Request ID: {RequestId}", errorModel.RequestId);

        return View(errorModel);
    }

    // Optional: About Us page
    public IActionResult About()
    {
        return View();
    }
}