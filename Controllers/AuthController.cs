using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MotiZone.Authorization;
using MotiZone.Models;
using MotiZone.Models.ViewModels;

namespace MotiZone.Controllers;

public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly JwtHelper _jwtHelper;

    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, JwtHelper jwtHelper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtHelper = jwtHelper;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
            return Conflict("Email is already registered.");

        var user = new User
        {
            UserName = model.Email,
            Email = model.Email,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(user, RolePolicy.Customer);

        return Ok(new { message = "User registered successfully." });
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || !user.IsActive)
            return Unauthorized("Invalid credentials.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!result.Succeeded)
            return Unauthorized("Invalid credentials.");

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtHelper.GenerateJwtToken(user, roles);

        return Ok(new { token });
    }
}
