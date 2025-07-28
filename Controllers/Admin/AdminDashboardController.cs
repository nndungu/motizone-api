using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MotiZone.Controllers.Admin;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("Admin/[controller]/[action]")]
public class AdminDashboardController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}