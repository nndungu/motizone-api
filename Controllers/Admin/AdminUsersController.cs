using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MotiZone.Models;

namespace MotiZone.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("Admin/[controller]/[action]")]
public class AdminUsersController : Controller
{
    private readonly UserManager<User> _userManager;

    public AdminUsersController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        var users = _userManager.Users.ToList();
        return View(users);
    }
}