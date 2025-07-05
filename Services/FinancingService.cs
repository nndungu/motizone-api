using Microsoft.EntityFrameworkCore;
using MotiZone.Data;
using MotiZone.Models;

namespace MotiZone.Services;

public class FinancingService
{
    private readonly ApplicationDbContext _context;

    public FinancingService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SubmitApplicationAsync(FinancingApplication app)
    {
        _context.FinancingApplications.Add(app);
        await _context.SaveChangesAsync();
    }

    public async Task<List<FinancingApplication>> GetUserApplicationsAsync(int userId)
    {
        return await _context.FinancingApplications
            .Where(a => a.UserId == userId)
            .ToListAsync();
    }
}