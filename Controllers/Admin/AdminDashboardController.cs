using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MotiZone.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("Admin/[controller]/[action]")]
public class AdminDashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}