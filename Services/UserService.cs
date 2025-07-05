using Microsoft.AspNetCore.Identity;
using MotiZone.Data;
using MotiZone.Models;

namespace MotiZone.Services;

public class UserService 
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public UserService(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await Task.FromResult(_userManager.Users.ToList());
    }

    public async Task<bool> ToggleUserStatusAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        user.LockoutEnabled = !user.LockoutEnabled;
        if (user.LockoutEnabled)
            user.LockoutEnd = DateTimeOffset.MaxValue;
        else
            user.LockoutEnd = null;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _userManager.FindByIdAsync(userId.ToString());
    }

    public async Task UpdateUserRoleAsync(User user)
    {
        var existingUser = await _userManager.FindByIdAsync(user.Id.ToString());
        if (existingUser != null)
        {
            existingUser.Role = user.Role;
            await _userManager.UpdateAsync(existingUser);
        }
    }

    public async Task<IdentityResult> RegisterUserAsync(User user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
    {
        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
    {
        return await _userManager.ConfirmEmailAsync(user, token);
    }

    public async Task<string> GeneratePasswordResetTokenAsync(User user)
    {
        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword)
    {
        return await _userManager.ResetPasswordAsync(user, token, newPassword);
    }

    public async Task<SignInResult> PasswordSignInAsync(string email, string password, bool rememberMe)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return SignInResult.Failed;

        return await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: false);
    }

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}
