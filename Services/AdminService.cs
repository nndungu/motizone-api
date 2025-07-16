using Microsoft.EntityFrameworkCore;
using MotiZone.Data;
using MotiZone.Models;
using MotiZone.Models.ViewModels;

namespace MotiZone.Services;

public class AdminService
{
    private readonly ApplicationDbContext _context;

    public AdminService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Payment>> FilterPaymentsAsync(AdminFilterViewModel filters)
    {
        var query = _context.Payments.AsQueryable();

        if (filters.UserId.HasValue)
            query = query.Where(p => p.UserId == filters.UserId);

        if (!string.IsNullOrEmpty(filters.Method))
            query = query.Where(p => p.PaymentMethod == filters.Method);

        if (filters.From.HasValue)
            query = query.Where(p => p.PaidAt >= filters.From);

        if (filters.To.HasValue)
            query = query.Where(p => p.PaidAt <= filters.To);

        return await query.OrderByDescending(p => p.PaidAt).ToListAsync();
    }
}