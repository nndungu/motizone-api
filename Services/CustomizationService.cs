using Microsoft.EntityFrameworkCore;
using MotiZone.Data;
using MotiZone.Models;

public class CustomizationService
{
    private readonly ApplicationDbContext _context;

    public CustomizationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Customization>> GetAvailableCustomizationsAsync()
    {
        return await _context.Customizations.ToListAsync();
    }

    public async Task<Customization?> GetCustomizationByIdAsync(int id)
    {
        return await _context.Customizations.FindAsync(id);
    }

    public async Task AddCustomizationAsync(Customization customization)
    {
        _context.Customizations.Add(customization);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCustomizationAsync(Customization customization)
    {
        _context.Customizations.Update(customization);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCustomizationAsync(int id)
    {
        var customization = await _context.Customizations.FindAsync(id);
        if (customization != null)
        {
            _context.Customizations.Remove(customization);
            await _context.SaveChangesAsync();
        }
    }
}